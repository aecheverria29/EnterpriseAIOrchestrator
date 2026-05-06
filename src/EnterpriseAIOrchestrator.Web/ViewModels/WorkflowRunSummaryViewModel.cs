namespace EnterpriseAIOrchestrator.Web.ViewModels;

public sealed record WorkflowRunSummaryViewModel(
    Guid RunId,
    string OriginalTitle,
    string BusinessUseCase,
    string WorkflowStatus,
    bool RequiresApprovalAction,
    string DefaultOwner,
    DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt);
