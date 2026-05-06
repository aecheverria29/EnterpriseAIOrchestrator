using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Domain.Aggregates;

namespace EnterpriseAIOrchestrator.Orchestration.Strategies;

public sealed class RuleBasedWorkRequestRoutingStrategy : IWorkRequestRoutingStrategy
{
    public Task<string> RouteAsync(
        WorkRequest workRequest,
        string classification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workRequest);
        ArgumentException.ThrowIfNullOrWhiteSpace(classification);

        cancellationToken.ThrowIfCancellationRequested();

        var route = classification switch
        {
            "AccessRequest" => "ITOperations",
            "FinanceRequest" => "Finance",
            "HrRequest" => "HumanResources",
            _ => "StandardProcessing"
        };

        return Task.FromResult(route);
    }
}
