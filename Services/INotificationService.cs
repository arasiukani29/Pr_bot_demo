namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public interface INotificationService
{
    IEnumerable<NotificationItem> GetAll(bool? unreadOnly = null);
    NotificationItem? GetById(Guid id);
    NotificationItem Create(CreateNotificationDto dto);
    bool MarkAsRead(Guid id);
    bool Delete(Guid id);
    int GetUnreadCount();
}
