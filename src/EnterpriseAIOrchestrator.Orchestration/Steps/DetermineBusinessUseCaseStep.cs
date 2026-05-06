using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Pipeline;
using EnterpriseAIOrchestrator.Domain.Enums;

namespace EnterpriseAIOrchestrator.Orchestration.Steps;

public sealed class DetermineBusinessUseCaseStep : IWorkRequestProcessingStep
{
    public string StepName => "DetermineBusinessUseCase";

    public Task<WorkRequestProcessingStepResult> ExecuteAsync(
        WorkRequestProcessingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        cancellationToken.ThrowIfCancellationRequested();

        context.BusinessUseCase = context.Classification switch
        {
            "AccessRequest" => BusinessUseCaseType.AccessManagement,
            "FinanceRequest" => BusinessUseCaseType.FinanceOperations,
            "HrRequest" => BusinessUseCaseType.HumanResources,
            _ => BusinessUseCaseType.GeneralOperations
        };

        context.AddAuditEntry(
            "BusinessUseCaseDetermined",
            $"Business use case mapped to '{context.BusinessUseCase}'.");
        context.StepsExecuted.Add(StepName);

        return Task.FromResult(new WorkRequestProcessingStepResult(
            StepName,
            "Success",
            "Business use case determined."));
    }
}
