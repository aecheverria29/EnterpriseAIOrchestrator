using EnterpriseAIOrchestrator.Application.Common;

namespace EnterpriseAIOrchestrator.Application.UseCases.ProcessWorkRequest;

public sealed record ProcessWorkRequestResult(
    Guid RunId,
    string OriginalTitle,
    string Classification,
    string AssignedRoute,
    string WorkflowStatus,
    string BusinessUseCase,
    bool RequiresManualReview,
    bool RequiresApprovalAction,
    string? ReviewDecision,
    int TargetSlaHours,
    string DefaultOwner,
    string FinalSummary,
    IReadOnlyCollection<WorkflowRunAuditEntry> AuditTrail,
    IReadOnlyCollection<string> ValidationMessages,
    IReadOnlyCollection<string> StepsExecuted,
    DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt);
