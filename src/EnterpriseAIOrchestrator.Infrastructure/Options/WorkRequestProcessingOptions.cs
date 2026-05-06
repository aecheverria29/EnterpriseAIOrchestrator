namespace EnterpriseAIOrchestrator.Infrastructure.Options;

public sealed class WorkRequestProcessingOptions
{
    public const string SectionName = "WorkRequestProcessing";

    public string ClassificationMode { get; set; } = "Rules";

    public string RoutingMode { get; set; } = "Rules";

    public bool AllowExternalAi { get; set; } = false;

    public bool PreferLocalProcessing { get; set; } = true;
}
