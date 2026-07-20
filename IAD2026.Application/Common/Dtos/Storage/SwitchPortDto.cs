using IAD2026.Application.Attributes;
using IAD2026.Application.Mappings;
namespace IAD2026.Application.Dtos.Storage;

[PaginatedResponse(
    ItemsProperty = "switchports",
    TotalProperty = "total_count")]
public class SwitchPortDto
{
    [JsonPath("switchport_id")]
    public int SwitchPortId { get; set; }

    [JsonPath("hwaddress")]
    public string HwAddress { get; set; } = string.Empty;

    [JsonPath("port")]
    public string Port { get; set; } = string.Empty;

    [JsonPath("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPath("is_connected")]
    public string IsConnected { get; set; } = string.Empty;
}