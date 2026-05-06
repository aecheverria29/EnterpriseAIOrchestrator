using EnterpriseAIOrchestrator.Application.Common;
using EnterpriseAIOrchestrator.Infrastructure.Options;
using EnterpriseAIOrchestrator.Infrastructure.Stores;
using EnterpriseAIOrchestrator.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseAIOrchestrator.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<WorkRequestProcessingOptions>()
            .Bind(configuration.GetSection(WorkRequestProcessingOptions.SectionName));
        services
            .AddOptions<UseCasePolicyOptions>()
            .Bind(configuration.GetSection(UseCasePolicyOptions.SectionName))
            .PostConfigure(options =>
            {
                options.Policies = BuildFinalPolicies(options.Policies);
            });
        services.AddSingleton<IWorkflowRunStore, InMemoryWorkflowRunStore>();

        return services;
    }

    private static IReadOnlyCollection<BusinessUseCasePolicy> BuildFinalPolicies(
        IReadOnlyCollection<BusinessUseCasePolicy>? configuredPolicies)
    {
        var policies = UseCasePolicyOptions.GetDefaultPolicies()
            .ToDictionary(
                policy => policy.BusinessUseCase,
                policy => new BusinessUseCasePolicy
                {
                    BusinessUseCase = policy.BusinessUseCase,
                    RequiresApprovalByDefault = policy.RequiresApprovalByDefault,
                    TargetSlaHours = policy.TargetSlaHours,
                    DefaultOwner = policy.DefaultOwner
                },
                StringComparer.Ordinal);

        if (configuredPolicies is null)
        {
            return policies.Values.ToArray();
        }

        foreach (var policy in configuredPolicies)
        {
            if (string.IsNullOrWhiteSpace(policy.BusinessUseCase))
            {
                continue;
            }

            policies[policy.BusinessUseCase] = new BusinessUseCasePolicy
            {
                BusinessUseCase = policy.BusinessUseCase.Trim(),
                RequiresApprovalByDefault = policy.RequiresApprovalByDefault,
                TargetSlaHours = policy.TargetSlaHours,
                DefaultOwner = policy.DefaultOwner
            };
        }

        return policies.Values.ToArray();
    }
}
