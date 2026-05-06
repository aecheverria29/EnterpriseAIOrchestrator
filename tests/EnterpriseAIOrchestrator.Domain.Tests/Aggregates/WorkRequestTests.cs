using EnterpriseAIOrchestrator.Domain.Aggregates;
using EnterpriseAIOrchestrator.Domain.Enums;
using EnterpriseAIOrchestrator.Domain.Exceptions;

namespace EnterpriseAIOrchestrator.Domain.Tests.Aggregates;

public sealed class WorkRequestTests
{
    [Fact]
    public void Constructor_CreatesValidWorkRequest()
    {
        var createdAt = new DateTimeOffset(2026, 4, 21, 12, 0, 0, TimeSpan.Zero);

        var workRequest = new WorkRequest(
            "  Access Request  ",
            "  Need access to reporting portal.  ",
            "  Finance  ",
            "  Jane Doe  ",
            RequestPriority.Medium,
            new[] { " reporting ", "finance" },
            createdAt: createdAt);

        Assert.Equal("Access Request", workRequest.Title);
        Assert.Equal("Need access to reporting portal.", workRequest.Description);
        Assert.Equal("Finance", workRequest.Department);
        Assert.Equal("Jane Doe", workRequest.RequestedBy);
        Assert.Equal(RequestPriority.Medium, workRequest.Priority);
        Assert.Equal(createdAt, workRequest.CreatedAt);
        Assert.Equal(new[] { "reporting", "finance" }, workRequest.Tags);
    }

    [Fact]
    public void Constructor_ThrowsWhenTitleIsEmpty()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new WorkRequest(
            "   ",
            "Valid description",
            "Operations",
            "John Doe",
            RequestPriority.Low));

        Assert.Equal("title is required.", exception.Message, ignoreCase: true);
    }

    [Fact]
    public void Constructor_NormalizesTagsRemovingEmptyAndDuplicates()
    {
        var workRequest = new WorkRequest(
            "Access Request",
            "Need access to reporting portal.",
            "Finance",
            "Jane Doe",
            RequestPriority.High,
            new string?[] { " finance ", "", "Finance", "  ", null, "Urgent" }!);

        Assert.Equal(new[] { "finance", "Urgent" }, workRequest.Tags);
    }
}
