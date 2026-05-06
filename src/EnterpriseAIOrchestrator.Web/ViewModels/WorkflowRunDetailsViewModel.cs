namespace EnterpriseAIOrchestrator.Web.ViewModels;

public sealed record WorkflowRunDetailsViewModel(
    WorkflowRunSummaryViewModel Summary,
    string Classification,
    string AssignedRoute,
    string? ReviewDecision,
    int TargetSlaHours,
    string FinalSummary,
    IReadOnlyCollection<string> ValidationMessages,
    IReadOnlyCollection<string> StepsExecuted,
    IReadOnlyCollection<WorkflowRunAuditEntryViewModel> AuditTrail,
    ReviewDecisionViewModel ReviewDecisionInput);
