namespace EnterpriseAIOrchestrator.Web.Models;

public sealed record ApiHealthStatus(
    bool IsAvailable,
    string Status,
    string Service,
    DateTimeOffset? UtcTime,
    string? ErrorMessage);
