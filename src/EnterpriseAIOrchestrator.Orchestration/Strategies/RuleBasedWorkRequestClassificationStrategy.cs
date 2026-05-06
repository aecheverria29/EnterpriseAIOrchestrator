using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Domain.Aggregates;

namespace EnterpriseAIOrchestrator.Orchestration.Strategies;

public sealed class RuleBasedWorkRequestClassificationStrategy : IWorkRequestClassificationStrategy
{
    private static readonly string[] AccessKeywords = ["vpn", "access", "account", "permissions"];
    private static readonly string[] FinanceKeywords = ["invoice", "payment", "purchase", "supplier"];
    private static readonly string[] HrKeywords = ["vacation", "leave", "absence"];

    public Task<string> ClassifyAsync(
        WorkRequest workRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workRequest);

        cancellationToken.ThrowIfCancellationRequested();

        var searchableContent = string.Concat(workRequest.Title, " ", workRequest.Description);

        var classification = ContainsAny(searchableContent, AccessKeywords)
            ? "AccessRequest"
            : ContainsAny(searchableContent, FinanceKeywords)
                ? "FinanceRequest"
                : ContainsAny(searchableContent, HrKeywords)
                    ? "HrRequest"
                    : "GeneralRequest";

        return Task.FromResult(classification);
    }

    private static bool ContainsAny(string content, IEnumerable<string> keywords)
    {
        foreach (var keyword in keywords)
        {
            if (content.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
