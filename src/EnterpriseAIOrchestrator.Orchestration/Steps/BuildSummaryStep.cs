using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Common;
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

        context.FinalSummary = WorkflowSummaryBuilder.Build(
            context.WorkRequest.Title,
            classification,
            assignedRoute,
            context.BusinessUseCase.ToString(),
            context.DefaultOwner,
            context.TargetSlaHours,
            context.WorkflowStatus.ToString());
        context.StepsExecuted.Add(StepName);

        return Task.FromResult(new WorkRequestProcessingStepResult(
            StepName,
            "Success",
            "Final summary built."));
    }
}
