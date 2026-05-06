namespace EnterpriseAIOrchestrator.Application.Pipeline;

public sealed record WorkRequestProcessingStepResult(
    string StepName,
    string Status,
    string? Message);
