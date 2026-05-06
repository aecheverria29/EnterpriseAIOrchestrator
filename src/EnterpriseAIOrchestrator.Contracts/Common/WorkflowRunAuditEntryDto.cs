namespace EnterpriseAIOrchestrator.Contracts.Common;

public sealed record WorkflowRunAuditEntryDto(
    Guid RunId,
    DateTimeOffset Timestamp,
    string EventType,
    string Message,
    string? PerformedBy);
