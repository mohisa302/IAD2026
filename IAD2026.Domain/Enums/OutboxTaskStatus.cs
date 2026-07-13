namespace IAD2026.Domain.Entities;

public enum OutboxTaskStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}

