using EnterpriseAIOrchestrator.Application.UseCases.ProcessWorkRequest;

namespace EnterpriseAIOrchestrator.Application.Abstractions;

public interface IWorkflowRunStore
{
    Task SaveAsync(ProcessWorkRequestResult result, CancellationToken cancellationToken = default);

    Task<ProcessWorkRequestResult?> GetByRunIdAsync(Guid runId, CancellationToken cancellationToken = default);

    Task UpdateAsync(ProcessWorkRequestResult result, CancellationToken cancellationToken = default);
}
