using EnterpriseAIOrchestrator.Domain.Aggregates;

namespace EnterpriseAIOrchestrator.Application.Abstractions;

public interface IWorkRequestRoutingStrategy
{
    Task<string> RouteAsync(
        WorkRequest workRequest,
        string classification,
        CancellationToken cancellationToken = default);
}
