namespace EnterpriseAIOrchestrator.Web.Models;

public sealed record ApiHealthResponse(
    string? Status,
    string? Service,
    DateTimeOffset? UtcTime);
