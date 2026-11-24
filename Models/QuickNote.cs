namespace DummyLoginApi.Models;

using System.ComponentModel.DataAnnotations;

public class CreateNoteRequestDto
{
    [Required(ErrorMessage = "Title is required.", AllowEmptyStrings = false)]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Title cannot consist solely of whitespace.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Content is required.", AllowEmptyStrings = false)]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Content cannot consist solely of whitespace.")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 2000 characters.")]
    public string Content { get; init; } = string.Empty;

    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
    public string Category { get; init; } = "General";
}

public class NoteResponseDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public class NoteItem
{
    public Guid Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
