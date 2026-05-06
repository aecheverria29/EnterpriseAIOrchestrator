using EnterpriseAIOrchestrator.Contracts.Common;

namespace EnterpriseAIOrchestrator.Contracts.Responses;

public sealed record WorkflowResultDto(
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
    IReadOnlyCollection<WorkflowRunAuditEntryDto> AuditTrail,
    IReadOnlyCollection<string> ValidationMessages,
    IReadOnlyCollection<string> StepsExecuted,
    DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt);
