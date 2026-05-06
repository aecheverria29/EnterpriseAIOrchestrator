using System.Diagnostics;
using EnterpriseAIOrchestrator.Web.Models;
using EnterpriseAIOrchestrator.Web.Services;
using EnterpriseAIOrchestrator.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAIOrchestrator.Web.Controllers;

public sealed class HomeController : Controller
{
    private readonly IEnterpriseAiOrchestratorApiClient _apiClient;

    public HomeController(IEnterpriseAiOrchestratorApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var health = await _apiClient.GetHealthAsync(cancellationToken);
        IReadOnlyCollection<WorkflowRunSummaryViewModel> recentRuns = Array.Empty<WorkflowRunSummaryViewModel>();

        if (health.IsAvailable)
        {
            try
            {
                var results = await _apiClient.GetRecentWorkRequestsAsync(20, cancellationToken);
                recentRuns = results.Select(result => new WorkflowRunSummaryViewModel(
                    result.RunId,
                    result.OriginalTitle,
                    result.BusinessUseCase,
                    result.WorkflowStatus,
                    result.RequiresApprovalAction,
                    result.DefaultOwner,
                    result.StartedAt,
                    result.CompletedAt)).ToArray();
            }
            catch (HttpRequestException)
            {
                recentRuns = Array.Empty<WorkflowRunSummaryViewModel>();
            }
        }

        var requestsInReview = recentRuns.Count(run => string.Equals(run.WorkflowStatus, "InReview", StringComparison.Ordinal));

        return View(new HomeIndexViewModel(
            health,
            requestsInReview,
            recentRuns.Count,
            recentRuns));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
