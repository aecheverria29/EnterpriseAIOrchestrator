using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Orchestration.Services;
using EnterpriseAIOrchestrator.Orchestration.Steps;
using EnterpriseAIOrchestrator.Orchestration.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseAIOrchestrator.Orchestration.DependencyInjection;

public static class OrchestrationServiceCollectionExtensions
{
    public static IServiceCollection AddOrchestrationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddScoped<IWorkRequestClassificationStrategy, RuleBasedWorkRequestClassificationStrategy>();
        services.AddScoped<IWorkRequestRoutingStrategy, RuleBasedWorkRequestRoutingStrategy>();
        services.AddScoped<IWorkRequestProcessingStep, ClassifyWorkRequestStep>();
        services.AddScoped<IWorkRequestProcessingStep, DetermineBusinessUseCaseStep>();
        services.AddScoped<IWorkRequestProcessingStep, ReviewDecisionStep>();
        services.AddScoped<IWorkRequestProcessingStep, BuildSummaryStep>();
        services.AddScoped<WorkRequestProcessingPipeline>();
        services.AddScoped<IWorkRequestProcessingService, PlaceholderWorkRequestProcessingService>();

        return services;
    }
}
