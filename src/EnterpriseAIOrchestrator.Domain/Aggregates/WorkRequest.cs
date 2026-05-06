using EnterpriseAIOrchestrator.Domain.Enums;
using EnterpriseAIOrchestrator.Domain.Exceptions;

namespace EnterpriseAIOrchestrator.Domain.Aggregates;

public sealed class WorkRequest
{
    public WorkRequest(
        string title,
        string description,
        string department,
        string requestedBy,
        RequestPriority priority,
        IReadOnlyCollection<string>? tags = null,
        Guid? id = null,
        DateTimeOffset? createdAt = null)
    {
        Id = id ?? Guid.NewGuid();
        Title = NormalizeRequired(title, nameof(title));
        Description = NormalizeRequired(description, nameof(description));
        Department = NormalizeRequired(department, nameof(department));
        RequestedBy = NormalizeRequired(requestedBy, nameof(requestedBy));
        Priority = priority;
        Tags = NormalizeTags(tags);
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }

    public string Title { get; }

    public string Description { get; }

    public string Department { get; }

    public string RequestedBy { get; }

    public RequestPriority Priority { get; }

    public IReadOnlyCollection<string> Tags { get; }

    public DateTimeOffset CreatedAt { get; }

    private static string NormalizeRequired(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{propertyName} is required.");
        }

        return value.Trim();
    }

    private static IReadOnlyCollection<string> NormalizeTags(IReadOnlyCollection<string>? tags)
    {
        if (tags is null || tags.Count == 0)
        {
            return Array.Empty<string>();
        }

        var normalizedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                continue;
            }

            normalizedTags.Add(tag.Trim());
        }

        return normalizedTags.Count == 0
            ? Array.Empty<string>()
            : normalizedTags.ToArray();
    }
}
