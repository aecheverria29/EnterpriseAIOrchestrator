namespace EnterpriseAIOrchestrator.Contracts.Requests;

public sealed record CreateWorkRequestDto(
    string Title,
    string Description,
    string Department,
    string RequestedBy,
    string Priority,
    IReadOnlyCollection<string>? Tags);
