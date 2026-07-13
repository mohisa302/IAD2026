using IAD2026.Shared.Models;

namespace IAD2026.Application.Options;

public class ExternalApiOptions
{
    public Dictionary<string, ExternalSystemOptions> Systems { get; set; } = new();
}

public class ExternalSystemOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public AuthType AuthType { get; set; }
    public string? ApiKey { get; set; }
    public string ApiKeyHeaderName { get; set; } = "X-Api-Key";   // ← Customizable
    public string? Username { get; set; }
    public string? Password { get; set; }
}