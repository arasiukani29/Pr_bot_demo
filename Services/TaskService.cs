namespace DummyLoginApi.Services;

using System.Collections.Concurrent;
using DummyLoginApi.Models;

public class TaskService : ITaskService
{
    private static readonly ConcurrentDictionary<Guid, TaskItem> _tasks = new();

    static TaskService()
    {
        SeedInitialTasks();
    }

    public IEnumerable<TaskItem> GetAll(TaskFilterDto filter, out int totalCount)
    {
        var query = _tasks.Values.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var search = filter.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(t =>
                t.Title.ToLowerInvariant().Contains(search) ||
                t.Description.ToLowerInvariant().Contains(search) ||
                (t.AssignedTo != null && t.AssignedTo.ToLowerInvariant().Contains(search))
            );
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(t => t.Status == filter.Status.Value);
        }

        if (filter.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == filter.Priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.AssignedTo))
        {
            query = query.Where(t =>
                t.AssignedTo != null &&
                t.AssignedTo.Equals(filter.AssignedTo.Trim(), StringComparison.OrdinalIgnoreCase)
            );
        }

        totalCount = query.Count();

        // Sorting
        query = (filter.SortBy?.ToLowerInvariant(), filter.SortDescending) switch
        {
            ("title", false) => query.OrderBy(t => t.Title),
            ("title", true) => query.OrderByDescending(t => t.Title),
            ("priority", false) => query.OrderBy(t => t.Priority),
            ("priority", true) => query.OrderByDescending(t => t.Priority),
            ("status", false) => query.OrderBy(t => t.Status),
            ("status", true) => query.OrderByDescending(t => t.Status),
            ("duedate", false) => query.OrderBy(t => t.DueDate ?? DateTime.MaxValue),
            ("duedate", true) => query.OrderByDescending(t => t.DueDate ?? DateTime.MinValue),
            (_, true) => query.OrderByDescending(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        // Paging
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);

        return query.Skip(page * pageSize).Take(pageSize).ToList();
    }

    public TaskItem? GetById(Guid id)
    {
        return _tasks.TryGetValue(id, out var task) ? task : null;
    }

    public TaskItem Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Status = dto.Status,
            Priority = dto.Priority,
            AssignedTo = string.IsNullOrWhiteSpace(dto.AssignedTo) ? null : dto.AssignedTo.Trim(),
            DueDate = dto.DueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _tasks[task.Id] = task;
        return task;
    }

    public TaskItem? Update(Guid id, UpdateTaskDto dto)
    {
        if (!_tasks.TryGetValue(id, out var existing))
        {
            return null;
        }

        existing.Title = dto.Title.Trim();
        existing.Description = dto.Description.Trim();
        existing.Status = dto.Status;
        existing.Priority = dto.Priority;
        existing.AssignedTo = string.IsNullOrWhiteSpace(dto.AssignedTo) ? null : dto.AssignedTo.Trim();
        existing.DueDate = dto.DueDate;
        existing.UpdatedAt = DateTime.UtcNow;

        _tasks[id] = existing;
        return existing;
    }

    public bool Delete(Guid id)
    {
        return _tasks.TryRemove(id, out _);
    }

    public TaskSummaryDto GetSummary()
    {
        var all = _tasks.Values.ToList();
        var now = DateTime.UtcNow;

        return new TaskSummaryDto
        {
            TotalTasks = all.Count,
            TodoCount = all.Count(t => t.Status == TaskItemStatus.Todo),
            InProgressCount = all.Count(t => t.Status == TaskItemStatus.InProgress),
            InReviewCount = all.Count(t => t.Status == TaskItemStatus.InReview),
            CompletedCount = all.Count(t => t.Status == TaskItemStatus.Completed),
            CriticalPriorityCount = all.Count(t => t.Priority == TaskPriority.Critical),
            OverdueCount = all.Count(t => t.DueDate.HasValue && t.DueDate.Value > now && t.Status != TaskItemStatus.Completed)
        };
    }

    private static void SeedInitialTasks()
    {
        var seedTasks = new[]
        {
            new TaskItem
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "Set up automated test pipeline",
                Description = "Configure CI workflow to run unit and integration tests on pull requests.",
                Status = TaskItemStatus.Completed,
                Priority = TaskPriority.High,
                AssignedTo = "alex.dev",
                DueDate = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new TaskItem
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "Implement JWT token refresh mechanism",
                Description = "Add sliding expiration and refresh token rotation in the authentication flow.",
                Status = TaskItemStatus.InProgress,
                Priority = TaskPriority.Critical,
                AssignedTo = "sarah.sec",
                DueDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddHours(-5)
            },
            new TaskItem
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Title = "Design responsive task dashboard UI",
                Description = "Create Angular standalone components for task management with mobile-first layout.",
                Status = TaskItemStatus.InReview,
                Priority = TaskPriority.Medium,
                AssignedTo = "emily.ui",
                DueDate = DateTime.UtcNow.AddDays(4),
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new TaskItem
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Title = "Add rate limiting middleware",
                Description = "Protect public endpoints against brute-force attacks and abuse.",
                Status = TaskItemStatus.Todo,
                Priority = TaskPriority.Low,
                AssignedTo = "alex.dev",
                DueDate = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = null
            }
        };

        foreach (var task in seedTasks)
        {
            _tasks[task.Id] = task;
        }
    }
}
