using System;

namespace IAD2026.Domain.Entities;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}