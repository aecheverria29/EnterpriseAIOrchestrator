using EnterpriseAIOrchestrator.Contracts.Requests;
using EnterpriseAIOrchestrator.Contracts.Responses;
using EnterpriseAIOrchestrator.Web.Models;

namespace EnterpriseAIOrchestrator.Web.Services;

public interface IEnterpriseAiOrchestratorApiClient
{
    Task<ApiHealthStatus> GetHealthAsync(CancellationToken cancellationToken = default);

    Task<WorkflowResultDto> CreateWorkRequestAsync(
        CreateWorkRequestDto request,
        CancellationToken cancellationToken = default);

    Task<WorkflowResultDto?> GetWorkRequestAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<WorkflowResultDto>> GetRecentWorkRequestsAsync(
        int count,
        CancellationToken cancellationToken = default);

    Task<WorkflowResultDto> ApproveAsync(
        Guid runId,
        ReviewDecisionRequestDto request,
        CancellationToken cancellationToken = default);

    Task<WorkflowResultDto> RejectAsync(
        Guid runId,
        ReviewDecisionRequestDto request,
        CancellationToken cancellationToken = default);
}
