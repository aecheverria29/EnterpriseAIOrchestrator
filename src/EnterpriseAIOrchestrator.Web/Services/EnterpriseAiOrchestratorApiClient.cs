using System.Net;
using System.Net.Http.Json;
using EnterpriseAIOrchestrator.Contracts.Requests;
using EnterpriseAIOrchestrator.Contracts.Responses;
using EnterpriseAIOrchestrator.Web.Models;

namespace EnterpriseAIOrchestrator.Web.Services;

public sealed class EnterpriseAiOrchestratorApiClient : IEnterpriseAiOrchestratorApiClient
{
    private readonly HttpClient _httpClient;

    public EnterpriseAiOrchestratorApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiHealthStatus> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiHealthResponse>("api/health", cancellationToken);

            return new ApiHealthStatus(
                true,
                response?.Status ?? "unknown",
                response?.Service ?? "EnterpriseAIOrchestrator.Api",
                response?.UtcTime,
                null);
        }
        catch (HttpRequestException exception)
        {
            return new ApiHealthStatus(false, "unavailable", "EnterpriseAIOrchestrator.Api", null, exception.Message);
        }
    }

    public async Task<WorkflowResultDto> CreateWorkRequestAsync(
        CreateWorkRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/workrequests", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await ReadWorkflowResultAsync(response, cancellationToken);
    }

    public async Task<WorkflowResultDto?> GetWorkRequestAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/workrequests/{runId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await ReadWorkflowResultAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<WorkflowResultDto>> GetRecentWorkRequestsAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<WorkflowResultDto>>(
            $"api/workrequests/recent?count={count}",
            cancellationToken) ?? Array.Empty<WorkflowResultDto>();
    }

    public async Task<WorkflowResultDto> ApproveAsync(
        Guid runId,
        ReviewDecisionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/workrequests/{runId}/approve", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await ReadWorkflowResultAsync(response, cancellationToken);
    }

    public async Task<WorkflowResultDto> RejectAsync(
        Guid runId,
        ReviewDecisionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/workrequests/{runId}/reject", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await ReadWorkflowResultAsync(response, cancellationToken);
    }

    private static async Task<WorkflowResultDto> ReadWorkflowResultAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        return await response.Content.ReadFromJsonAsync<WorkflowResultDto>(cancellationToken)
            ?? throw new InvalidOperationException("The API returned an empty workflow result.");
    }
}
