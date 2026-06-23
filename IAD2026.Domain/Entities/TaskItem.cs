namespace IAD2026.Domain.Entities;

/// <summary>
/// Core business entity representing a Task.
/// </summary>
public class TaskItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public List<object> DomainEvents { get; } = new();

    private TaskItem() { }

    public TaskItem(string title)
    {
        SetTitle(title);
        IsCompleted = false;

    }

    public void Complete()
    {
        IsCompleted = true;
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");

        if (title.Length > 200)
            throw new ArgumentException("Title is too long");

        Title = title;
    }
}