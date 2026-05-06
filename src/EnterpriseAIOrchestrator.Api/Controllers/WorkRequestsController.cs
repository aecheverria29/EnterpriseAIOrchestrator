using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Common;
using EnterpriseAIOrchestrator.Application.UseCases.ProcessWorkRequest;
using EnterpriseAIOrchestrator.Contracts.Common;
using EnterpriseAIOrchestrator.Contracts.Requests;
using EnterpriseAIOrchestrator.Contracts.Responses;
using EnterpriseAIOrchestrator.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAIOrchestrator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class WorkRequestsController : ControllerBase
{
    private readonly IWorkRequestProcessingService _workRequestProcessingService;
    private readonly IWorkflowRunStore _workflowRunStore;

    public WorkRequestsController(
        IWorkRequestProcessingService workRequestProcessingService,
        IWorkflowRunStore workflowRunStore)
    {
        _workRequestProcessingService = workRequestProcessingService;
        _workflowRunStore = workflowRunStore;
    }

    [HttpPost]
    public async Task<ActionResult<WorkflowResultDto>> Post(
        CreateWorkRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new ProcessWorkRequestCommand(
                request.Title,
                request.Description,
                request.Department,
                request.RequestedBy,
                request.Priority,
                request.Tags);

            var result = await _workRequestProcessingService.ProcessAsync(command, cancellationToken);
            return Ok(ToDto(result));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ApiErrorDto(
                "invalid_request",
                exception.Message,
                null));
        }
        catch (DomainValidationException exception)
        {
            return BadRequest(new ApiErrorDto(
                "domain_validation_error",
                exception.Message,
                null));
        }
    }

    [HttpGet("{runId:guid}")]
    public async Task<ActionResult<WorkflowResultDto>> GetByRunId(Guid runId, CancellationToken cancellationToken)
    {
        var result = await _workflowRunStore.GetByRunIdAsync(runId, cancellationToken);

        if (result is null)
        {
            return NotFound(new ApiErrorDto(
                "run_not_found",
                $"Workflow run '{runId}' was not found.",
                null));
        }

        return Ok(ToDto(result));
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IReadOnlyCollection<WorkflowResultDto>>> GetRecent(
        [FromQuery] int count = 20,
        CancellationToken cancellationToken = default)
    {
        var results = await _workflowRunStore.GetRecentAsync(count, cancellationToken);
        return Ok(results.Select(ToDto).ToArray());
    }

    [HttpPost("{runId:guid}/approve")]
    public Task<ActionResult<WorkflowResultDto>> Approve(
        Guid runId,
        ReviewDecisionRequestDto request,
        CancellationToken cancellationToken)
    {
        return ApplyReviewDecisionAsync(runId, "Approved", request, cancellationToken);
    }

    [HttpPost("{runId:guid}/reject")]
    public Task<ActionResult<WorkflowResultDto>> Reject(
        Guid runId,
        ReviewDecisionRequestDto request,
        CancellationToken cancellationToken)
    {
        return ApplyReviewDecisionAsync(runId, "Rejected", request, cancellationToken);
    }

    private async Task<ActionResult<WorkflowResultDto>> ApplyReviewDecisionAsync(
        Guid runId,
        string reviewDecision,
        ReviewDecisionRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PerformedBy))
        {
            return BadRequest(new ApiErrorDto(
                "invalid_request",
                "PerformedBy is required.",
                null));
        }

        var existingResult = await _workflowRunStore.GetByRunIdAsync(runId, cancellationToken);

        if (existingResult is null)
        {
            return NotFound(new ApiErrorDto(
                "run_not_found",
                $"Workflow run '{runId}' was not found.",
                null));
        }

        if (!string.Equals(existingResult.WorkflowStatus, "InReview", StringComparison.Ordinal))
        {
            return BadRequest(new ApiErrorDto(
            "invalid_workflow_state",
                $"Workflow run '{runId}' cannot be marked as '{reviewDecision}' from status '{existingResult.WorkflowStatus}'.",
                null));
        }

        var auditTrail = existingResult.AuditTrail.ToList();
        auditTrail.Add(new WorkflowRunAuditEntry(
            runId,
            DateTimeOffset.UtcNow,
            reviewDecision,
            BuildReviewMessage(reviewDecision, request.Comment),
            request.PerformedBy.Trim()));

        var updatedResult = existingResult with
        {
            WorkflowStatus = reviewDecision,
            ReviewDecision = string.IsNullOrWhiteSpace(request.Comment)
                ? reviewDecision
                : $"{reviewDecision}: {request.Comment.Trim()}",
            RequiresApprovalAction = false
            ,
            AuditTrail = auditTrail
        };

        await _workflowRunStore.UpdateAsync(updatedResult, cancellationToken);

        return Ok(ToDto(updatedResult));
    }

    private static WorkflowResultDto ToDto(ProcessWorkRequestResult result)
    {
        return new WorkflowResultDto(
            result.RunId,
            result.OriginalTitle,
            result.Classification,
            result.AssignedRoute,
            result.WorkflowStatus,
            result.BusinessUseCase,
            result.RequiresManualReview,
            result.RequiresApprovalAction,
            result.ReviewDecision,
            result.TargetSlaHours,
            result.DefaultOwner,
            result.FinalSummary,
            result.AuditTrail.Select(entry => new WorkflowRunAuditEntryDto(
                entry.RunId,
                entry.Timestamp,
                entry.EventType,
                entry.Message,
                entry.PerformedBy)).ToArray(),
            result.ValidationMessages,
            result.StepsExecuted,
            result.StartedAt,
            result.CompletedAt);
    }

    private static string BuildReviewMessage(string reviewDecision, string? comment)
    {
        return string.IsNullOrWhiteSpace(comment)
            ? $"Workflow run {reviewDecision.ToLowerInvariant()}."
            : $"Workflow run {reviewDecision.ToLowerInvariant()}. Comment: {comment.Trim()}";
    }
}
