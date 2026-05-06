using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Common;
using EnterpriseAIOrchestrator.Application.Pipeline;
using EnterpriseAIOrchestrator.Domain.Enums;
using Microsoft.Extensions.Options;

namespace EnterpriseAIOrchestrator.Orchestration.Steps;

public sealed class ReviewDecisionStep : IWorkRequestProcessingStep
{
    private readonly IReadOnlyDictionary<string, BusinessUseCasePolicy> _policies;

    public ReviewDecisionStep(IOptions<UseCasePolicyOptions> useCasePolicyOptions)
    {
        ArgumentNullException.ThrowIfNull(useCasePolicyOptions);

        var duplicateUseCases = useCasePolicyOptions.Value.Policies
            .Where(policy => !string.IsNullOrWhiteSpace(policy.BusinessUseCase))
            .GroupBy(policy => policy.BusinessUseCase, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(key => key, StringComparer.Ordinal)
            .ToArray();

        if (duplicateUseCases.Length > 0)
        {
            throw new InvalidOperationException(
                $"Duplicate business use case policies found: {string.Join(", ", duplicateUseCases)}");
        }

        _policies = useCasePolicyOptions.Value.Policies.ToDictionary(
            policy => policy.BusinessUseCase,
            StringComparer.Ordinal);
    }

    public string StepName => "ReviewDecision";

    public Task<WorkRequestProcessingStepResult> ExecuteAsync(
        WorkRequestProcessingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        cancellationToken.ThrowIfCancellationRequested();

        var businessUseCase = context.BusinessUseCase.ToString();
        if (!_policies.TryGetValue(businessUseCase, out var policy))
        {
            throw new InvalidOperationException($"No business use case policy found for '{businessUseCase}'.");
        }

        context.TargetSlaHours = policy.TargetSlaHours;
        context.DefaultOwner = policy.DefaultOwner;
        context.RequiresManualReview = context.BusinessUseCase switch
        {
            BusinessUseCaseType.AccessManagement => policy.RequiresApprovalByDefault,
            BusinessUseCaseType.FinanceOperations => policy.RequiresApprovalByDefault,
            BusinessUseCaseType.HumanResources => context.WorkRequest.Priority is RequestPriority.High or RequestPriority.Critical,
            BusinessUseCaseType.GeneralOperations => context.WorkRequest.Priority is RequestPriority.Critical,
            _ => false
        };
        context.WorkflowStatus = context.RequiresManualReview
            ? WorkRequestWorkflowStatus.InReview
            : WorkRequestWorkflowStatus.Completed;
        context.AddAuditEntry(
            context.RequiresManualReview ? "SentToReview" : "Completed",
            context.RequiresManualReview
                ? $"Request sent to review. Target SLA: {context.TargetSlaHours}h. Default owner: '{context.DefaultOwner}'."
                : $"Request completed automatically. Target SLA: {context.TargetSlaHours}h. Default owner: '{context.DefaultOwner}'.");
        context.StepsExecuted.Add(StepName);

        return Task.FromResult(new WorkRequestProcessingStepResult(
            StepName,
            "Success",
            "Manual review decision evaluated."));
    }
}
