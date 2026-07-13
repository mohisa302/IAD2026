using Microsoft.EntityFrameworkCore;
using IAD2026.Application.Interfaces;
using IAD2026.Domain.Entities;

namespace IAD2026.Persistence.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly AppDbContext _context;

    public OutboxRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<OutboxTask>> GetPendingTasksAsync(int batchSize, CancellationToken cancellationToken)
    {
        return _context.TaskQueue
            .Where(t => t.Status == OutboxTaskStatus.Pending && t.RetryCount < 3)
            .OrderBy(t => t.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<OutboxTask?> GetTaskByIdAsync(string taskId, CancellationToken cancellationToken)
    {
        return await _context.TaskQueue.FindAsync(new object[] { taskId }, cancellationToken);
    }

    public Task UpdateTaskAsync(OutboxTask task, CancellationToken cancellationToken)
    {
        _context.TaskQueue.Update(task);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}