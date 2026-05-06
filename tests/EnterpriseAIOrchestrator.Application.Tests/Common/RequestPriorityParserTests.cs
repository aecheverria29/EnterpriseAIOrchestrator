using EnterpriseAIOrchestrator.Application.Common;
using EnterpriseAIOrchestrator.Domain.Enums;

namespace EnterpriseAIOrchestrator.Application.Tests.Common;

public sealed class RequestPriorityParserTests
{
    [Theory]
    [InlineData("low", RequestPriority.Low)]
    [InlineData("medium", RequestPriority.Medium)]
    [InlineData("high", RequestPriority.High)]
    [InlineData("critical", RequestPriority.Critical)]
    [InlineData("LoW", RequestPriority.Low)]
    public void Parse_ReturnsExpectedPriority(string input, RequestPriority expected)
    {
        var result = RequestPriorityParser.Parse(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_ThrowsForInvalidValue()
    {
        var exception = Assert.Throws<ArgumentException>(() => RequestPriorityParser.Parse("urgent"));

        Assert.Contains("Invalid priority", exception.Message);
    }
}
