using EnterpriseAIOrchestrator.Domain.Aggregates;
using EnterpriseAIOrchestrator.Domain.Enums;
using EnterpriseAIOrchestrator.Orchestration.Strategies;

namespace EnterpriseAIOrchestrator.Application.Tests.Strategies;

public sealed class RuleBasedWorkRequestClassificationStrategyTests
{
    private readonly RuleBasedWorkRequestClassificationStrategy _strategy = new();

    [Theory]
    [InlineData("VPN request", "Need secure connection for remote work.", "AccessRequest")]
    [InlineData("Supplier follow-up", "Invoice payment is overdue.", "FinanceRequest")]
    [InlineData("Vacation planning", "Leave requested for next week.", "HrRequest")]
    [InlineData("Office request", "Need a new chair for the branch office.", "GeneralRequest")]
    public async Task ClassifyAsync_ReturnsExpectedClassification(
        string title,
        string description,
        string expectedClassification)
    {
        var workRequest = CreateWorkRequest(title, description);

        var classification = await _strategy.ClassifyAsync(workRequest);

        Assert.Equal(expectedClassification, classification);
    }

    private static WorkRequest CreateWorkRequest(string title, string description)
    {
        return new WorkRequest(
            title,
            description,
            "Operations",
            "Jane Doe",
            RequestPriority.Medium);
    }
}
