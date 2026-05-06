using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Pipeline;

namespace EnterpriseAIOrchestrator.Orchestration.Steps;

public sealed class BuildSummaryStep : IWorkRequestProcessingStep
{
    public string StepName => "BuildSummary";

    public Task<WorkRequestProcessingStepResult> ExecuteAsync(
        WorkRequestProcessingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        cancellationToken.ThrowIfCancellationRequested();

        var classification = context.Classification ?? "UnclassifiedRequest";
        var assignedRoute = context.AssignedRoute ?? "ManualTriage";

        context.FinalSummary =
            $"Request '{context.WorkRequest.Title}' was normalized, classified as '{classification}', routed to '{assignedRoute}', mapped to '{context.BusinessUseCase}', assigned to '{context.DefaultOwner}' with SLA {context.TargetSlaHours}h, and is currently '{context.WorkflowStatus}'.";
        context.StepsExecuted.Add(StepName);

        return Task.FromResult(new WorkRequestProcessingStepResult(
            StepName,
            "Success",
            "Final summary built."));
    }
}
