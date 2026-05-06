using EnterpriseAIOrchestrator.Api.Controllers;
using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Common;
using EnterpriseAIOrchestrator.Application.UseCases.ProcessWorkRequest;
using EnterpriseAIOrchestrator.Contracts.Common;
using EnterpriseAIOrchestrator.Contracts.Requests;
using EnterpriseAIOrchestrator.Contracts.Responses;
using EnterpriseAIOrchestrator.Infrastructure.Stores;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAIOrchestrator.IntegrationTests.Api;

public sealed class ControllerSmokeTests
{
    [Fact]
    public void HealthController_Get_ReturnsOkResult()
    {
        var controller = new HealthController();

        var result = controller.Get();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task WorkRequestsController_Post_ReturnsWorkflowResult()
    {
        var store = new InMemoryWorkflowRunStore();
        var expectedResult = new ProcessWorkRequestResult(
            Guid.NewGuid(),
            "VPN Access",
            "AccessRequest",
            "ITOperations",
            "Completed",
            "AccessManagement",
            false,
            false,
            null,
            8,
            "ITOperations",
            "Summary",
            [new WorkflowRunAuditEntry(Guid.NewGuid(), DateTimeOffset.UtcNow, "RunCreated", "Created", null)],
            Array.Empty<string>(),
            ["ClassifyRequest", "DetermineBusinessUseCase", "ReviewDecision", "BuildSummary"],
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        var controller = new WorkRequestsController(new StubWorkRequestProcessingService(expectedResult), store);
        var request = new CreateWorkRequestDto(
            "VPN Access",
            "Need VPN access for a remote employee.",
            "Operations",
            "Jane Doe",
            "medium",
            ["remote"]);

        var actionResult = await controller.Post(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<WorkflowResultDto>(okResult.Value);

        Assert.Equal("AccessRequest", response.Classification);
        Assert.Equal("ITOperations", response.AssignedRoute);
        Assert.Equal("Completed", response.WorkflowStatus);
    }

    [Fact]
    public async Task WorkRequestsController_Approve_ChangesInReviewToApproved()
    {
        var store = new InMemoryWorkflowRunStore();
        var run = CreateInReviewRun();
        await store.SaveAsync(run);

        var controller = new WorkRequestsController(new StubWorkRequestProcessingService(run), store);

        var actionResult = await controller.Approve(
            run.RunId,
            new ReviewDecisionRequestDto("manager@corp.local", "Approved after supervisor review"),
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<WorkflowResultDto>(okResult.Value);

        Assert.Equal("Approved", response.WorkflowStatus);
        Assert.Equal("Approved: Approved after supervisor review", response.ReviewDecision);
        Assert.False(response.RequiresApprovalAction);
        Assert.Contains("Approved", response.FinalSummary);
        Assert.DoesNotContain("InReview", response.FinalSummary);
        Assert.Contains(response.AuditTrail, entry => entry.EventType == "Approved" && entry.PerformedBy == "manager@corp.local");

        var storedRun = await store.GetByRunIdAsync(run.RunId);
        Assert.NotNull(storedRun);
        Assert.Equal(response.FinalSummary, storedRun.FinalSummary);
        Assert.Equal(response.CompletedAt, storedRun.CompletedAt);
    }

    [Fact]
    public async Task WorkRequestsController_Reject_ChangesInReviewToRejected()
    {
        var store = new InMemoryWorkflowRunStore();
        var run = CreateInReviewRun();
        await store.SaveAsync(run);

        var controller = new WorkRequestsController(new StubWorkRequestProcessingService(run), store);

        var actionResult = await controller.Reject(
            run.RunId,
            new ReviewDecisionRequestDto("manager@corp.local", "Missing supporting documents"),
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<WorkflowResultDto>(okResult.Value);

        Assert.Equal("Rejected", response.WorkflowStatus);
        Assert.Equal("Rejected: Missing supporting documents", response.ReviewDecision);
        Assert.False(response.RequiresApprovalAction);
        Assert.Contains("Rejected", response.FinalSummary);
        Assert.DoesNotContain("InReview", response.FinalSummary);
        Assert.Contains(response.AuditTrail, entry => entry.EventType == "Rejected" && entry.PerformedBy == "manager@corp.local");

        var storedRun = await store.GetByRunIdAsync(run.RunId);
        Assert.NotNull(storedRun);
        Assert.Equal(response.FinalSummary, storedRun.FinalSummary);
        Assert.Equal(response.CompletedAt, storedRun.CompletedAt);
    }

    [Fact]
    public async Task WorkRequestsController_GetByRunId_ReturnsNotFoundForMissingRun()
    {
        var controller = new WorkRequestsController(
            new StubWorkRequestProcessingService(CreateInReviewRun()),
            new InMemoryWorkflowRunStore());

        var actionResult = await controller.GetByRunId(Guid.NewGuid(), CancellationToken.None);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.IsType<ApiErrorDto>(notFoundResult.Value);
    }

    [Fact]
    public async Task WorkRequestsController_GetRecent_ReturnsRunsOrderedByStartedAt()
    {
        var store = new InMemoryWorkflowRunStore();
        var olderRun = CreateInReviewRun() with { StartedAt = DateTimeOffset.UtcNow.AddMinutes(-10) };
        var newerRun = CreateInReviewRun() with { StartedAt = DateTimeOffset.UtcNow };
        await store.SaveAsync(olderRun);
        await store.SaveAsync(newerRun);

        var controller = new WorkRequestsController(new StubWorkRequestProcessingService(newerRun), store);

        var actionResult = await controller.GetRecent(20, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsAssignableFrom<IReadOnlyCollection<WorkflowResultDto>>(okResult.Value);
        Assert.Equal([newerRun.RunId, olderRun.RunId], response.Select(run => run.RunId));
    }

    [Fact]
    public async Task WorkRequestsController_Approve_ReturnsBadRequestWhenPerformedByMissing()
    {
        var store = new InMemoryWorkflowRunStore();
        var run = CreateInReviewRun();
        await store.SaveAsync(run);

        var controller = new WorkRequestsController(new StubWorkRequestProcessingService(run), store);

        var actionResult = await controller.Approve(
            run.RunId,
            new ReviewDecisionRequestDto(" ", "Approved"),
            CancellationToken.None);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.IsType<ApiErrorDto>(badRequestResult.Value);
    }

    private sealed class StubWorkRequestProcessingService : IWorkRequestProcessingService
    {
        private readonly ProcessWorkRequestResult _result;

        public StubWorkRequestProcessingService(ProcessWorkRequestResult result)
        {
            _result = result;
        }

        public Task<ProcessWorkRequestResult> ProcessAsync(
            ProcessWorkRequestCommand command,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_result);
        }
    }

    private static ProcessWorkRequestResult CreateInReviewRun()
    {
        return new ProcessWorkRequestResult(
            Guid.NewGuid(),
            "Access request",
            "AccessRequest",
            "ITOperations",
            "InReview",
            "AccessManagement",
            true,
            true,
            null,
            8,
            "ITOperations",
            "Summary",
            [new WorkflowRunAuditEntry(Guid.NewGuid(), DateTimeOffset.UtcNow, "RunCreated", "Created", null)],
            Array.Empty<string>(),
            ["ClassifyRequest", "DetermineBusinessUseCase", "ReviewDecision", "BuildSummary"],
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
    }
}
