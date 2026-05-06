using EnterpriseAIOrchestrator.Domain.Enums;

namespace EnterpriseAIOrchestrator.Application.Common;

public sealed class UseCasePolicyOptions
{
    public const string SectionName = "UseCasePolicies";

    public IReadOnlyCollection<BusinessUseCasePolicy> Policies { get; set; } = Array.Empty<BusinessUseCasePolicy>();

    public static IReadOnlyCollection<BusinessUseCasePolicy> GetDefaultPolicies()
    {
        return
        [
            new()
            {
                BusinessUseCase = BusinessUseCaseType.AccessManagement.ToString(),
                RequiresApprovalByDefault = true,
                TargetSlaHours = 8,
                DefaultOwner = "ITOperations"
            },
            new()
            {
                BusinessUseCase = BusinessUseCaseType.FinanceOperations.ToString(),
                RequiresApprovalByDefault = true,
                TargetSlaHours = 12,
                DefaultOwner = "Finance"
            },
            new()
            {
                BusinessUseCase = BusinessUseCaseType.HumanResources.ToString(),
                RequiresApprovalByDefault = false,
                TargetSlaHours = 24,
                DefaultOwner = "HumanResources"
            },
            new()
            {
                BusinessUseCase = BusinessUseCaseType.GeneralOperations.ToString(),
                RequiresApprovalByDefault = false,
                TargetSlaHours = 48,
                DefaultOwner = "Operations"
            }
        ];
    }
}
