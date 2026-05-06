namespace EnterpriseAIOrchestrator.Web.ViewModels;

public sealed record RecentRequestsViewModel(IReadOnlyCollection<WorkflowRunSummaryViewModel> Runs);
