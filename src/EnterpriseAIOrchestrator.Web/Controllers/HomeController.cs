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
        return View(new HomeIndexViewModel(health));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
