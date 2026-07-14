using IAD2026.Domain.Enums;

namespace IAD2026.Domain.Entities;

public class DcimSnapshot: BaseEntity
{
    public string JsonBody { get; set; } = string.Empty;

    public DcimType DcimType { get; set; }

    public DateTime CurrentDate { get; set; }
}