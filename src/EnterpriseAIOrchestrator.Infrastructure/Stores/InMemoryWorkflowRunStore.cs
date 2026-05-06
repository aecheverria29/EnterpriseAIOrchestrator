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

    public Task<IReadOnlyCollection<ProcessWorkRequestResult>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var boundedCount = Math.Clamp(count, 1, 100);
        var results = _runs.Values
            .OrderByDescending(run => run.StartedAt)
            .ThenByDescending(run => run.CompletedAt)
            .Take(boundedCount)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<ProcessWorkRequestResult>>(results);
    }

    public Task UpdateAsync(ProcessWorkRequestResult result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        cancellationToken.ThrowIfCancellationRequested();

        _runs[result.RunId] = result;

        return Task.CompletedTask;
    }
}
