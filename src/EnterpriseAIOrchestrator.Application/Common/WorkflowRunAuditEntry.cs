namespace EnterpriseAIOrchestrator.Application.Common;

public sealed record WorkflowRunAuditEntry(
    Guid RunId,
    DateTimeOffset Timestamp,
    string EventType,
    string Message,
    string? PerformedBy);
