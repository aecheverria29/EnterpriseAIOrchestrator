using EnterpriseAIOrchestrator.Application.Pipeline;

namespace EnterpriseAIOrchestrator.Application.Abstractions;

public interface IWorkRequestProcessingStep
{
    string StepName { get; }

    Task<WorkRequestProcessingStepResult> ExecuteAsync(
        WorkRequestProcessingContext context,
        CancellationToken cancellationToken = default);
}
