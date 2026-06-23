using IAD2026.Domain.Entities;

namespace IAD2026.Application.Abstractions;

/// <summary>
/// Abstraction for Task persistence
/// </summary>
public interface ITaskRepository
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken);

    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<TaskItem>> GetAllAsync(CancellationToken cancellationToken);
}