using EnterpriseAIOrchestrator.Application.Abstractions;
using EnterpriseAIOrchestrator.Application.Pipeline;

namespace EnterpriseAIOrchestrator.Orchestration.Steps;

public sealed class ClassifyWorkRequestStep : IWorkRequestProcessingStep
{
    private readonly IWorkRequestClassificationStrategy _classificationStrategy;
    private readonly IWorkRequestRoutingStrategy _routingStrategy;

    public ClassifyWorkRequestStep(
        IWorkRequestClassificationStrategy classificationStrategy,
        IWorkRequestRoutingStrategy routingStrategy)
    {
        _classificationStrategy = classificationStrategy ?? throw new ArgumentNullException(nameof(classificationStrategy));
        _routingStrategy = routingStrategy ?? throw new ArgumentNullException(nameof(routingStrategy));
    }

    public string StepName => "ClassifyRequest";

    public async Task<WorkRequestProcessingStepResult> ExecuteAsync(
        WorkRequestProcessingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        cancellationToken.ThrowIfCancellationRequested();

        var classification = await _classificationStrategy.ClassifyAsync(context.WorkRequest, cancellationToken);
        var assignedRoute = await _routingStrategy.RouteAsync(context.WorkRequest, classification, cancellationToken);

        context.Classification = classification;
        context.AssignedRoute = assignedRoute;
        context.AddAuditEntry(
            "Classified",
            $"Request classified as '{classification}' and routed to '{assignedRoute}'.");
        context.StepsExecuted.Add(StepName);

        return new WorkRequestProcessingStepResult(
            StepName,
            "Success",
            "Request classified and routed.");
    }
}
