using System.Collections.Concurrent;
using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.UseCases.ProcessWorkRequest;

namespace EnterpriseAIOrchestrator.Infrastructure.Stores;

public sealed class InMemoryWorkflowRunStore : IWorkflowRunStore
{
    private readonly ConcurrentDictionary<Guid, ProcessWorkRequestResult> _runs = new();

    public Task SaveAsync(ProcessWorkRequestResult result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        cancellationToken.ThrowIfCancellationRequested();

        _runs[result.RunId] = result;

        return Task.CompletedTask;
    }

    public Task<ProcessWorkRequestResult?> GetByRunIdAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _runs.TryGetValue(runId, out var result);

        return Task.FromResult(result);
    }

    public Task UpdateAsync(ProcessWorkRequestResult result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        cancellationToken.ThrowIfCancellationRequested();

        _runs[result.RunId] = result;

        return Task.CompletedTask;
    }
}
