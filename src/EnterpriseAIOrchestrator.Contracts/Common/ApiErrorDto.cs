namespace EnterpriseAIOrchestrator.Contracts.Common;

public sealed record ApiErrorDto(
    string Code,
    string Message,
    IReadOnlyCollection<string>? Details);
