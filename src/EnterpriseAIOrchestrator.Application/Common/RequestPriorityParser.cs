using EnterpriseAIOrchestrator.Domain.Enums;

namespace EnterpriseAIOrchestrator.Application.Common;

public static class RequestPriorityParser
{
    public static RequestPriority Parse(string priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
        {
            throw new ArgumentException("Priority value is required.", nameof(priority));
        }

        return priority.Trim().ToLowerInvariant() switch
        {
            "low" => RequestPriority.Low,
            "medium" => RequestPriority.Medium,
            "high" => RequestPriority.High,
            "critical" => RequestPriority.Critical,
            _ => throw new ArgumentException(
                $"Invalid priority '{priority}'. Allowed values are: low, medium, high, critical.",
                nameof(priority))
        };
    }
}
