using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Pipeline;
using EnterpriseAIOrchestrator.Application.UseCases.ProcessWorkRequest;
using EnterpriseAIOrchestrator.Domain.Aggregates;
using EnterpriseAIOrchestrator.Domain.Enums;

namespace EnterpriseAIOrchestrator.Orchestration.Services;

public sealed class WorkRequestProcessingPipeline
{
    private readonly IEnumerable<IWorkRequestProcessingStep> _steps;

    public WorkRequestProcessingPipeline(IEnumerable<IWorkRequestProcessingStep> steps)
    {
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
    }

    public async Task<ProcessWorkRequestResult> ExecuteAsync(
        WorkRequest workRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workRequest);

        var context = new WorkRequestProcessingContext(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            workRequest);
        context.AddAuditEntry(
            "RunCreated",
            $"Workflow run created for request '{workRequest.Title}'.");

        foreach (var step in _steps)
        {
            var stepResult = await step.ExecuteAsync(context, cancellationToken);

            if (!string.Equals(stepResult.Status, "Success", StringComparison.OrdinalIgnoreCase))
            {
                context.ValidationMessages.Add(
                    stepResult.Message ?? $"Step '{step.StepName}' completed with status '{stepResult.Status}'.");
            }
        }

        var completedAt = DateTimeOffset.UtcNow;
        var classification = context.Classification ?? "UnclassifiedRequest";
        var assignedRoute = context.AssignedRoute ?? "ManualTriage";
        var workflowStatus = context.WorkflowStatus;
        var requiresApprovalAction = workflowStatus == WorkRequestWorkflowStatus.InReview;
        var finalSummary = context.FinalSummary
            ?? $"Request '{context.WorkRequest.Title}' completed pipeline execution with fallback values.";

        return new ProcessWorkRequestResult(
            context.RunId,
            context.WorkRequest.Title,
            classification,
            assignedRoute,
            workflowStatus.ToString(),
            context.BusinessUseCase.ToString(),
            context.RequiresManualReview,
            requiresApprovalAction,
            context.ReviewDecision,
            context.TargetSlaHours,
            context.DefaultOwner,
            finalSummary,
            context.AuditTrail,
            context.ValidationMessages,
            context.StepsExecuted,
            context.StartedAt,
            completedAt);
    }
}
