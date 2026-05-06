namespace EnterpriseAIOrchestrator.Application.Common;

public sealed class BusinessUseCasePolicy
{
    public string BusinessUseCase { get; set; } = string.Empty;

    public bool RequiresApprovalByDefault { get; set; }

    public int TargetSlaHours { get; set; }

    public string DefaultOwner { get; set; } = string.Empty;
}
