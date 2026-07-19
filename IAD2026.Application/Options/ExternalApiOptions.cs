using IAD2026.Domain.Enums;
using IAD2026.Shared.Models;

namespace IAD2026.Application.Options;


public class ExternalSystemOptions
{
    public string BaseUrl { get; set; } = string.Empty;

    public AuthType AuthType { get; set; }

    public string? ApiKey { get; set; }

    public string ApiKeyHeaderName { get; set; } = "X-Api-Key";

    public string? Username { get; set; }

    public string? Password { get; set; }

}
public class DcimSystemOptions : ExternalSystemOptions
{
    public int Offset { get; set; }

    public int Limit { get; set; }

    public List<DcimEndpointOption> Endpoints { get; set; } = [];
}
public class DcimEndpointOption
{
    public DcimType DcimType { get; set; }

    public string ServicePath { get; set; } = string.Empty;
}
public class ExternalApiOptions
{
    public ExternalSystemOptions Icare { get; set; } = new();

    public DcimSystemOptions DCIM { get; set; } = new();
}