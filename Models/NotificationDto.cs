namespace DummyLoginApi.Models;

using System.ComponentModel.DataAnnotations;

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Alert
}

public class NotificationItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateNotificationDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters.")]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Title cannot consist solely of whitespace.")]
    public string Title { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Message is required.")]
    [StringLength(500, MinimumLength = 2, ErrorMessage = "Message must be between 2 and 500 characters.")]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Message cannot consist solely of whitespace.")]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.Info;
}
