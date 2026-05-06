namespace EnterpriseAIOrchestrator.Contracts.Events;

public sealed record WorkflowProgressEventDto(
    Guid RunId,
    string StepName,
    string Status,
    DateTimeOffset Timestamp,
    string? Message);
