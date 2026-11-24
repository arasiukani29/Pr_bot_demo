namespace DummyLoginApi.Models;

using System.ComponentModel.DataAnnotations;

public class CreateNotificationRequestDto
{
    [Required(ErrorMessage = "Title is required.", AllowEmptyStrings = false)]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Title cannot consist solely of whitespace.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Message is required.", AllowEmptyStrings = false)]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Message cannot consist solely of whitespace.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 500 characters.")]
    public string Message { get; init; } = string.Empty;

    [StringLength(30, ErrorMessage = "Type cannot exceed 30 characters.")]
    public string Type { get; init; } = "Info";

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string? RecipientEmail { get; init; }
}

public class NotificationResponseDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string? RecipientEmail { get; init; }
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class NotificationItem
{
    public Guid Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? RecipientEmail { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; init; }
}
