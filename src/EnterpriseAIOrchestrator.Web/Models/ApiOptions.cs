namespace EnterpriseAIOrchestrator.Web.Models;

public sealed class ApiOptions
{
    public string ApiBaseUrl { get; set; } = "http://localhost:5181";

    public Uri GetBaseUri()
    {
        if (!Uri.TryCreate(ApiBaseUrl, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException("ApiBaseUrl must be an absolute URL.");
        }

        return uri;
    }
}
