using EnterpriseAIOrchestrator.Contracts.Requests;
using EnterpriseAIOrchestrator.Contracts.Responses;
using EnterpriseAIOrchestrator.Web.Services;
using EnterpriseAIOrchestrator.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAIOrchestrator.Web.Controllers;

public sealed class RequestsController : Controller
{
    private readonly IEnterpriseAiOrchestratorApiClient _apiClient;

    public RequestsController(IEnterpriseAiOrchestratorApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateWorkRequestViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWorkRequestViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _apiClient.CreateWorkRequestAsync(
                new CreateWorkRequestDto(
                    model.Title.Trim(),
                    model.Description.Trim(),
                    model.Department.Trim(),
                    model.RequestedBy.Trim(),
                    model.Priority.Trim(),
                    ParseTags(model.Tags)),
                cancellationToken);

            return RedirectToAction(nameof(Details), new { runId = result.RunId });
        }
        catch (HttpRequestException exception)
        {
            ModelState.AddModelError(string.Empty, $"The API rejected the request: {exception.Message}");
            return View(model);
        }
    }

    [HttpGet("Requests/Details/{runId:guid}")]
    public async Task<IActionResult> Details(Guid runId, CancellationToken cancellationToken)
    {
        var result = await _apiClient.GetWorkRequestAsync(runId, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return View(ToDetailsViewModel(result));
    }

    [HttpPost("Requests/Details/{runId:guid}/approve")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid runId, ReviewDecisionViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await DetailsWithReviewErrorAsync(runId, cancellationToken);
        }

        await _apiClient.ApproveAsync(
            runId,
            new ReviewDecisionRequestDto(model.PerformedBy.Trim(), model.Comment?.Trim()),
            cancellationToken);

        return RedirectToAction(nameof(Details), new { runId });
    }

    [HttpPost("Requests/Details/{runId:guid}/reject")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid runId, ReviewDecisionViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await DetailsWithReviewErrorAsync(runId, cancellationToken);
        }

        await _apiClient.RejectAsync(
            runId,
            new ReviewDecisionRequestDto(model.PerformedBy.Trim(), model.Comment?.Trim()),
            cancellationToken);

        return RedirectToAction(nameof(Details), new { runId });
    }

    [HttpGet]
    public IActionResult Lookup()
    {
        return View(new LookupRequestViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Lookup(LookupRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { runId = model.RunId });
    }

    [HttpGet]
    public async Task<IActionResult> Recent(CancellationToken cancellationToken)
    {
        var results = await _apiClient.GetRecentWorkRequestsAsync(20, cancellationToken);
        var model = new RecentRequestsViewModel(results.Select(ToSummaryViewModel).ToArray());

        return View(model);
    }

    private async Task<IActionResult> DetailsWithReviewErrorAsync(Guid runId, CancellationToken cancellationToken)
    {
        var result = await _apiClient.GetWorkRequestAsync(runId, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        ModelState.AddModelError(string.Empty, "PerformedBy is required to submit a review decision.");
        return View("Details", ToDetailsViewModel(result));
    }

    private static IReadOnlyCollection<string>? ParseTags(string? tags)
    {
        if (string.IsNullOrWhiteSpace(tags))
        {
            return null;
        }

        return tags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    private static WorkflowRunDetailsViewModel ToDetailsViewModel(WorkflowResultDto result)
    {
        return new WorkflowRunDetailsViewModel(
            ToSummaryViewModel(result),
            result.Classification,
            result.AssignedRoute,
            result.ReviewDecision,
            result.TargetSlaHours,
            result.FinalSummary,
            result.ValidationMessages,
            result.StepsExecuted,
            result.AuditTrail.Select(entry => new WorkflowRunAuditEntryViewModel(
                entry.Timestamp,
                entry.EventType,
                entry.Message,
                entry.PerformedBy)).ToArray(),
            new ReviewDecisionViewModel());
    }

    private static WorkflowRunSummaryViewModel ToSummaryViewModel(WorkflowResultDto result)
    {
        return new WorkflowRunSummaryViewModel(
            result.RunId,
            result.OriginalTitle,
            result.BusinessUseCase,
            result.WorkflowStatus,
            result.RequiresApprovalAction,
            result.DefaultOwner,
            result.StartedAt,
            result.CompletedAt);
    }
}
