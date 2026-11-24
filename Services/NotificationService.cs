namespace DummyLoginApi.Services;

using System.Collections.Concurrent;
using DummyLoginApi.Models;

public interface INotificationService
{
    IReadOnlyList<NotificationResponseDto> GetAll(bool? unreadOnly = null);
    NotificationResponseDto? GetById(Guid id);
    NotificationResponseDto Create(CreateNotificationRequestDto dto);
    bool MarkAsRead(Guid id);
    bool Delete(Guid id);
}

public class NotificationService : INotificationService
{
    private static readonly ConcurrentDictionary<Guid, NotificationItem> _notifications = new();

    public IReadOnlyList<NotificationResponseDto> GetAll(bool? unreadOnly = null)
    {
        var query = _notifications.Values.AsEnumerable();

        if (unreadOnly == true)
        {
            query = query.Where(n => !n.IsRead);
        }

        return query.OrderByDescending(n => n.CreatedAt).Select(MapToDto).ToList();
    }

    public NotificationResponseDto? GetById(Guid id)
    {
        if (_notifications.TryGetValue(id, out var notification))
        {
            return MapToDto(notification);
        }

        return null;
    }

    public NotificationResponseDto Create(CreateNotificationRequestDto dto)
    {
        var notification = new NotificationItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Message = dto.Message.Trim(),
            Type = string.IsNullOrWhiteSpace(dto.Type) ? "Info" : dto.Type.Trim(),
            RecipientEmail = string.IsNullOrWhiteSpace(dto.RecipientEmail)
                ? null
                : dto.RecipientEmail.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
        };

        _notifications[notification.Id] = notification;
        return MapToDto(notification);
    }

    public bool MarkAsRead(Guid id)
    {
        if (_notifications.TryGetValue(id, out var notification))
        {
            notification.IsRead = true;
            return true;
        }

        return false;
    }

    public bool Delete(Guid id)
    {
        return _notifications.TryRemove(id, out _);
    }

    private static NotificationResponseDto MapToDto(NotificationItem item) =>
        new()
        {
            Id = item.Id,
            Title = item.Title,
            Message = item.Message,
            Type = item.Type,
            RecipientEmail = item.RecipientEmail,
            IsRead = item.IsRead,
            CreatedAt = item.CreatedAt,
        };
}
