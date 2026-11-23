namespace DummyLoginApi.Models;

using System.ComponentModel.DataAnnotations;

public class CreateAnnouncementDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 120 characters.")]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Title cannot consist solely of whitespace.")]
    public string Title { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Content is required.")]
    [StringLength(2000, MinimumLength = 5, ErrorMessage = "Content must be between 5 and 2000 characters.")]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Content cannot consist solely of whitespace.")]
    public string Content { get; set; } = string.Empty;

    public AnnouncementCategory Category { get; set; } = AnnouncementCategory.General;

    public bool IsPinned { get; set; } = false;

    public DateTime? ExpiresAt { get; set; }
}
