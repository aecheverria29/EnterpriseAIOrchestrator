using EnterpriseAIOrchestrator.Application.Common;
using EnterpriseAIOrchestrator.Orchestration.Steps;
using Microsoft.Extensions.Options;

namespace EnterpriseAIOrchestrator.Application.Tests.Steps;

public sealed class ReviewDecisionStepTests
{
    [Fact]
    public void Constructor_ThrowsClearException_WhenBusinessUseCasePoliciesAreDuplicated()
    {
        var options = Options.Create(new UseCasePolicyOptions
        {
            Policies =
            [
                new BusinessUseCasePolicy
                {
                    BusinessUseCase = "AccessManagement",
                    RequiresApprovalByDefault = true,
                    TargetSlaHours = 8,
                    DefaultOwner = "ITOperations"
                },
                new BusinessUseCasePolicy
                {
                    BusinessUseCase = "AccessManagement",
                    RequiresApprovalByDefault = true,
                    TargetSlaHours = 12,
                    DefaultOwner = "Security"
                }
            ]
        });

        var exception = Assert.Throws<InvalidOperationException>(() => new ReviewDecisionStep(options));

        Assert.Equal(
            "Duplicate business use case policies found: AccessManagement",
            exception.Message);
    }
}
