namespace IAD2026.Domain.Entities;

public class OutboxTask : BaseEntity
{
    public string TaskType { get; set; } = default!;   // e.g., "SmsNsotification", "DataCleanup"
    public string ReferenceId { get; set; } = default!; // Unique identifier to maintain O(1) constraints
    public OutboxTaskStatus Status { get; set; }
    public string? Payload { get; set; }                // Serialized JSON arguments
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
}