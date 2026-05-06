using EnterpriseAIOrchestrator.Domain.Aggregates;

namespace EnterpriseAIOrchestrator.Application.Abstractions;

public interface IWorkRequestClassificationStrategy
{
    Task<string> ClassifyAsync(
        WorkRequest workRequest,
        CancellationToken cancellationToken = default);
}
