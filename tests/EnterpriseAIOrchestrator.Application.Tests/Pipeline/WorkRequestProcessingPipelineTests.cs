using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Common;
using EnterpriseAIOrchestrator.Domain.Aggregates;
using EnterpriseAIOrchestrator.Domain.Enums;
using EnterpriseAIOrchestrator.Orchestration.Services;
using EnterpriseAIOrchestrator.Orchestration.Steps;
using EnterpriseAIOrchestrator.Orchestration.Strategies;
using Microsoft.Extensions.Options;

namespace EnterpriseAIOrchestrator.Application.Tests.Pipeline;

public sealed class WorkRequestProcessingPipelineTests
{
    private static readonly IReadOnlyList<IWorkRequestProcessingStep> OrderedSteps =
    [
        new ClassifyWorkRequestStep(
            new RuleBasedWorkRequestClassificationStrategy(),
            new RuleBasedWorkRequestRoutingStrategy()),
        new DetermineBusinessUseCaseStep(),
        new ReviewDecisionStep(Options.Create(new UseCasePolicyOptions
        {
            Policies = UseCasePolicyOptions.GetDefaultPolicies()
        })),
        new BuildSummaryStep()
    ];

    [Fact]
    public async Task ExecuteAsync_SetsManualReviewToTrueForHighPriority()
    {
        var pipeline = new WorkRequestProcessingPipeline(OrderedSteps);
        var workRequest = CreateWorkRequest(RequestPriority.High);

        var result = await pipeline.ExecuteAsync(workRequest);

        Assert.True(result.RequiresManualReview);
        Assert.Equal("InReview", result.WorkflowStatus);
        Assert.True(result.RequiresApprovalAction);
    }

    [Fact]
    public async Task ExecuteAsync_GeneralOperationsLow_DoesNotRequireManualReview()
    {
        var pipeline = new WorkRequestProcessingPipeline(OrderedSteps);
        var workRequest = CreateWorkRequest(
            RequestPriority.Low,
            title: "Office request",
            description: "Need a new chair for the branch office.");

        var result = await pipeline.ExecuteAsync(workRequest);

        Assert.False(result.RequiresManualReview);
        Assert.Equal("Completed", result.WorkflowStatus);
        Assert.False(result.RequiresApprovalAction);
        Assert.Equal("GeneralOperations", result.BusinessUseCase);
    }

    [Fact]
    public async Task ExecuteAsync_BuildsExpectedSummary()
    {
        var pipeline = new WorkRequestProcessingPipeline(OrderedSteps);
        var workRequest = CreateWorkRequest(RequestPriority.Medium);

        var result = await pipeline.ExecuteAsync(workRequest);

        Assert.Equal(
            "Request 'Access Request' was normalized, classified as 'AccessRequest', routed to 'ITOperations', mapped to 'AccessManagement', assigned to 'ITOperations' with SLA 8h, and is currently 'InReview'.",
            result.FinalSummary);
    }

    [Fact]
    public async Task ExecuteAsync_PreservesStepOrder()
    {
        var pipeline = new WorkRequestProcessingPipeline(OrderedSteps);
        var workRequest = CreateWorkRequest(RequestPriority.Critical);

        var result = await pipeline.ExecuteAsync(workRequest);

        Assert.Equal(
            new[] { "ClassifyRequest", "DetermineBusinessUseCase", "ReviewDecision", "BuildSummary" },
            result.StepsExecuted);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesAuditTrailForProcessedRun()
    {
        var pipeline = new WorkRequestProcessingPipeline(OrderedSteps);
        var workRequest = CreateWorkRequest(RequestPriority.High);

        var result = await pipeline.ExecuteAsync(workRequest);

        Assert.Contains(result.AuditTrail, entry => entry.EventType == "RunCreated");
        Assert.Contains(result.AuditTrail, entry => entry.EventType == "Classified");
        Assert.Contains(result.AuditTrail, entry => entry.EventType == "BusinessUseCaseDetermined");
        Assert.Contains(result.AuditTrail, entry => entry.EventType == "SentToReview");
    }

    [Fact]
    public async Task ExecuteAsync_AccessManagement_RequiresApprovalByDefault()
    {
        var pipeline = new WorkRequestProcessingPipeline(OrderedSteps);
        var workRequest = CreateWorkRequest(RequestPriority.Low);

        var result = await pipeline.ExecuteAsync(workRequest);

        Assert.Equal("AccessManagement", result.BusinessUseCase);
        Assert.True(result.RequiresManualReview);
        Assert.Equal("InReview", result.WorkflowStatus);
    }

    [Fact]
    public async Task ExecuteAsync_AssignsSlaAndOwnerFromPolicy()
    {
        var pipeline = new WorkRequestProcessingPipeline(OrderedSteps);
        var workRequest = CreateWorkRequest(RequestPriority.Low);

        var result = await pipeline.ExecuteAsync(workRequest);

        Assert.Equal(8, result.TargetSlaHours);
        Assert.Equal("ITOperations", result.DefaultOwner);
    }

    private static WorkRequest CreateWorkRequest(
        RequestPriority priority,
        string title = "Access Request",
        string description = "Need access to reporting portal.")
    {
        return new WorkRequest(
            title,
            description,
            "Finance",
            "Jane Doe",
            priority,
            ["finance"]);
    }
}
