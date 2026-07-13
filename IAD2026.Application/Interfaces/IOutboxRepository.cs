using IAD2026.Domain.Entities;

namespace IAD2026.Application.Interfaces;

public interface IOutboxRepository
{
    Task<List<OutboxTask>> GetTasksByTypeAsync(string type, int batchSize, CancellationToken cancellationToken);
    Task<OutboxTask?> GetTaskByIdAsync(string taskId, CancellationToken cancellationToken);
    Task UpdateTaskAsync(OutboxTask task, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}