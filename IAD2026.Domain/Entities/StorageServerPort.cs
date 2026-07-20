using IAD2026.Domain.Enums;

namespace IAD2026.Domain.Entities;

public class StorageServerPort: BaseEntity
{
    public string HostName { get; set; }
    public string DeviceId { get; set; }
    public string HwAddress { get; set; }
}