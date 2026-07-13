using Microsoft.Extensions.Logging;
using Hangfire;
using IAD2026.Application.Interfaces;
using IAD2026.Domain.Entities;

namespace IAD2026.BackgroundJobs.Jobs;

public class OutboxProcessorJob
{
    private readonly IOutboxRepository _repository;
    private readonly Dictionary<string, ITaskExecutor> _executors;
    private readonly ILogger<OutboxProcessorJob> _logger;

    public OutboxProcessorJob(
        IOutboxRepository repository,
        IEnumerable<ITaskExecutor> executors,
        ILogger<OutboxProcessorJob> logger)
    {
        _repository = repository;
        _logger = logger;
        _executors = executors.ToDictionary(e => e.TaskType, StringComparer.OrdinalIgnoreCase);
    }

    // Update this signature
    public async Task DistributePendingTasksAsync(string taskType, CancellationToken cancellationToken)
    {
        // Update the repository call to filter by type
        var pendingTasks = await _repository.GetTasksByTypeAsync(taskType, 100, cancellationToken);

        if (!pendingTasks.Any()) return;

        foreach (var task in pendingTasks)
        {
            task.Status = OutboxTaskStatus.Processing;
            await _repository.UpdateTaskAsync(task, cancellationToken);
        }
        await _repository.SaveChangesAsync(cancellationToken);

        var backgroundClient = new BackgroundJobClient();
        foreach (var task in pendingTasks)
        {
            // Enqueue individual execution
            backgroundClient.Enqueue<OutboxProcessorJob>(job => job.ProcessSingleTaskAsync(task.Id, CancellationToken.None));
        }
    }

    public async Task ProcessSingleTaskAsync(string taskId, CancellationToken cancellationToken)
    {
        var task = await _repository.GetTaskByIdAsync(taskId, cancellationToken);
        if (task == null) return;

        if (!_executors.TryGetValue(task.TaskType, out var executor))
        {
            _logger.LogError("No task executor registered for type: {Type}", task.TaskType);
            task.Status = OutboxTaskStatus.Failed;
            task.ErrorMessage = $"Executor missing for '{task.TaskType}'";
            await _repository.UpdateTaskAsync(task, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return;
        }

        try
        {
            await executor.ExecuteAsync(task.Payload, cancellationToken);
            task.Status = OutboxTaskStatus.Completed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Execution failed for task ID: {Id}", task.Id);
            task.RetryCount++;
            task.ErrorMessage = ex.Message;
            task.Status = task.RetryCount >= 3 ? OutboxTaskStatus.Failed : OutboxTaskStatus.Pending;
        }
        finally
        {
            await _repository.UpdateTaskAsync(task, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}