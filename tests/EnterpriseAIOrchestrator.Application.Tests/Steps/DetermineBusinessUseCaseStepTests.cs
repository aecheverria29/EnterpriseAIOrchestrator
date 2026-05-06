using EnterpriseAIOrchestrator.Application.Pipeline;
using EnterpriseAIOrchestrator.Domain.Aggregates;
using EnterpriseAIOrchestrator.Domain.Enums;
using EnterpriseAIOrchestrator.Orchestration.Steps;

namespace EnterpriseAIOrchestrator.Application.Tests.Steps;

public sealed class DetermineBusinessUseCaseStepTests
{
    [Theory]
    [InlineData("AccessRequest", BusinessUseCaseType.AccessManagement)]
    [InlineData("FinanceRequest", BusinessUseCaseType.FinanceOperations)]
    [InlineData("HrRequest", BusinessUseCaseType.HumanResources)]
    [InlineData("GeneralRequest", BusinessUseCaseType.GeneralOperations)]
    public async Task ExecuteAsync_MapsClassificationToBusinessUseCase(
        string classification,
        BusinessUseCaseType expectedUseCase)
    {
        var step = new DetermineBusinessUseCaseStep();
        var context = new WorkRequestProcessingContext(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            new WorkRequest("Title", "Description", "Operations", "Jane Doe", RequestPriority.Low))
        {
            Classification = classification
        };

        await step.ExecuteAsync(context);

        Assert.Equal(expectedUseCase, context.BusinessUseCase);
        Assert.Contains("DetermineBusinessUseCase", context.StepsExecuted);
    }
}
