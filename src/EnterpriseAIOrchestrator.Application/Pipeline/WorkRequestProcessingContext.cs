using EnterpriseAIOrchestrator.Domain.Aggregates;
using EnterpriseAIOrchestrator.Domain.Enums;
using EnterpriseAIOrchestrator.Application.Common;

namespace EnterpriseAIOrchestrator.Application.Pipeline;

public sealed class WorkRequestProcessingContext
{
    public WorkRequestProcessingContext(Guid runId, DateTimeOffset startedAt, WorkRequest workRequest)
    {
        RunId = runId;
        StartedAt = startedAt;
        WorkRequest = workRequest ?? throw new ArgumentNullException(nameof(workRequest));
        WorkflowStatus = WorkRequestWorkflowStatus.Pending;
        BusinessUseCase = BusinessUseCaseType.GeneralOperations;
        AuditTrail = new List<WorkflowRunAuditEntry>();
        ValidationMessages = new List<string>();
        StepsExecuted = new List<string>();
    }

    public Guid RunId { get; }

    public DateTimeOffset StartedAt { get; }

    public WorkRequest WorkRequest { get; }

    public string? Classification { get; set; }

    public string? AssignedRoute { get; set; }

    public WorkRequestWorkflowStatus WorkflowStatus { get; set; }

    public BusinessUseCaseType BusinessUseCase { get; set; }

    public bool RequiresManualReview { get; set; }

    public string? ReviewDecision { get; set; }

    public int TargetSlaHours { get; set; }

    public string DefaultOwner { get; set; } = string.Empty;

    public string? FinalSummary { get; set; }

    public List<WorkflowRunAuditEntry> AuditTrail { get; }

    public List<string> ValidationMessages { get; }

    public List<string> StepsExecuted { get; }

    public void AddAuditEntry(string eventType, string message, string? performedBy = null)
    {
        AuditTrail.Add(new WorkflowRunAuditEntry(
            RunId,
            DateTimeOffset.UtcNow,
            eventType,
            message,
            performedBy));
    }
}
