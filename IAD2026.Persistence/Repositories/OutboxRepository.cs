using Microsoft.EntityFrameworkCore;
using IAD2026.Application.Interfaces;
using IAD2026.Domain.Entities;
using IAD2026.Domain.Enums;

namespace IAD2026.Persistence.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly AppDbContext _context;

    public OutboxRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<OutboxTask>> GetTasksByTypeAsync(string type, int batchSize, CancellationToken cancellationToken)
    {
        return await _context.TaskQueue
            .Where(t => t.TaskType == type &&
                        t.Status == OutboxTaskStatus.Pending &&
                        t.RetryCount < 3)
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