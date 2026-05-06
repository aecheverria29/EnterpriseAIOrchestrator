namespace EnterpriseAIOrchestrator.Web.ViewModels;

public sealed record WorkflowRunAuditEntryViewModel(
    DateTimeOffset Timestamp,
    string EventType,
    string Message,
    string? PerformedBy);
