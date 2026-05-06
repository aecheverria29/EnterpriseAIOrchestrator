using EnterpriseAIOrchestrator.Web.Models;

namespace EnterpriseAIOrchestrator.Web.ViewModels;

public sealed record HomeIndexViewModel(
    ApiHealthStatus ApiHealth,
    int RequestsInReview,
    int RecentRunCount,
    IReadOnlyCollection<WorkflowRunSummaryViewModel> RecentRuns);
