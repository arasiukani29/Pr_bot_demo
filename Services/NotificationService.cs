namespace DummyLoginApi.Services;

using System.Collections.Concurrent;
using DummyLoginApi.Models;

public class NotificationService : INotificationService
{
    private static readonly ConcurrentDictionary<Guid, NotificationItem> _notifications = new();

    static NotificationService()
    {
        SeedNotifications();
    }

    public IEnumerable<NotificationItem> GetAll(bool? unreadOnly = null)
    {
        var query = _notifications.Values.AsQueryable();

        if (unreadOnly.HasValue && unreadOnly.Value)
        {
            query = query.Where(n => !n.IsRead);
        }

        return query.OrderByDescending(n => n.CreatedAt).ToList();
    }

    public NotificationItem? GetById(Guid id)
    {
        return _notifications.TryGetValue(id, out var item) ? item : null;
    }

    public NotificationItem Create(CreateNotificationDto dto)
    {
        var item = new NotificationItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Message = dto.Message.Trim(),
            Type = dto.Type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _notifications[item.Id] = item;
        return item;
    }

    public bool MarkAsRead(Guid id)
    {
        if (!_notifications.TryGetValue(id, out var item))
        {
            return false;
        }

        item.IsRead = true;
        _notifications[id] = item;
        return true;
    }

    public bool Delete(Guid id)
    {
        return _notifications.TryRemove(id, out _);
    }

    public int GetUnreadCount()
    {
        return _notifications.Values.Count(n => !n.IsRead);
    }

    private static void SeedNotifications()
    {
        var samples = new[]
        {
            new NotificationItem
            {
                Id = Guid.Parse("aaaaaaa1-1111-1111-1111-111111111111"),
                Title = "Welcome to the Platform",
                Message = "Your account has been set up successfully. Explore the features!",
                Type = NotificationType.Success,
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            },
            new NotificationItem
            {
                Id = Guid.Parse("bbbbbbb2-2222-2222-2222-222222222222"),
                Title = "System Maintenance Scheduled",
                Message = "Scheduled maintenance will occur this weekend at 02:00 UTC.",
                Type = NotificationType.Warning,
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            new NotificationItem
            {
                Id = Guid.Parse("ccccccc3-3333-3333-3333-333333333333"),
                Title = "Security Policy Update",
                Message = "Please review the revised password expiration policy.",
                Type = NotificationType.Info,
                IsRead = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        foreach (var item in samples)
        {
            _notifications[item.Id] = item;
        }
    }
}
