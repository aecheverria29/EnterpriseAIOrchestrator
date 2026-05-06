namespace EnterpriseAIOrchestrator.Application.UseCases.ProcessWorkRequest;

public sealed record ProcessWorkRequestCommand(
    string Title,
    string Description,
    string Department,
    string RequestedBy,
    string Priority,
    IReadOnlyCollection<string>? Tags);
