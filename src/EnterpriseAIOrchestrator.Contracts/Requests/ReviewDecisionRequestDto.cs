namespace EnterpriseAIOrchestrator.Contracts.Requests;

public sealed record ReviewDecisionRequestDto(
    string PerformedBy,
    string? Comment);
