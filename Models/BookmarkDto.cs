namespace DummyLoginApi.Models;

using System.ComponentModel.DataAnnotations;

public class CreateBookmarkRequestDto
{
    [Required(ErrorMessage = "Title is required.", AllowEmptyStrings = false)]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Title cannot consist solely of whitespace.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "URL is required.", AllowEmptyStrings = false)]
    [Url(ErrorMessage = "URL must be a valid HTTP or HTTPS URL.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "URL must not exceed 500 characters.")]
    public string Url { get; init; } = string.Empty;

    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
    public string Category { get; init; } = "General";
}

public class BookmarkResponseDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public class BookmarkItem
{
    public Guid Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
