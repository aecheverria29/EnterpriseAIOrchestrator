using EnterpriseAIOrchestrator.Domain.Aggregates;
using EnterpriseAIOrchestrator.Domain.Enums;
using EnterpriseAIOrchestrator.Orchestration.Strategies;

namespace EnterpriseAIOrchestrator.Application.Tests.Strategies;

public sealed class RuleBasedWorkRequestRoutingStrategyTests
{
    private readonly RuleBasedWorkRequestRoutingStrategy _strategy = new();

    [Theory]
    [InlineData("AccessRequest", "ITOperations")]
    [InlineData("FinanceRequest", "Finance")]
    [InlineData("HrRequest", "HumanResources")]
    [InlineData("GeneralRequest", "StandardProcessing")]
    public async Task RouteAsync_ReturnsExpectedRoute(string classification, string expectedRoute)
    {
        var workRequest = new WorkRequest(
            "Any request",
            "Any description",
            "Operations",
            "Jane Doe",
            RequestPriority.Low);

        var route = await _strategy.RouteAsync(workRequest, classification);

        Assert.Equal(expectedRoute, route);
    }
}
