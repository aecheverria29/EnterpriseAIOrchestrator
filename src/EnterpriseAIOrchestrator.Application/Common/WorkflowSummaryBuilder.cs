namespace EnterpriseAIOrchestrator.Application.Common;

public static class WorkflowSummaryBuilder
{
    public static string Build(
        string title,
        string classification,
        string assignedRoute,
        string businessUseCase,
        string defaultOwner,
        int targetSlaHours,
        string workflowStatus)
    {
        return
            $"Request '{title}' was normalized, classified as '{classification}', routed to '{assignedRoute}', mapped to '{businessUseCase}', assigned to '{defaultOwner}' with SLA {targetSlaHours}h, and is currently '{workflowStatus}'.";
    }
}
