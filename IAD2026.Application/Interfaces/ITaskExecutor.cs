namespace IAD2026.Application.Interfaces;

public interface ITaskExecutor
{
    string TaskType { get; } // Matches OutboxTask.TaskType
    Task ExecuteAsync(string? payload, CancellationToken cancellationToken);
}