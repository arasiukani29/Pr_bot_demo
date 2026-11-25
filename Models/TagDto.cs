namespace DummyLoginApi.Models;

using System.ComponentModel.DataAnnotations;

public class CreateTagRequestDto
{
    [Required(ErrorMessage = "Tag name is required.", AllowEmptyStrings = false)]
    [RegularExpression(@".*\S+.*", ErrorMessage = "Tag name cannot consist solely of whitespace.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Tag name must be between 1 and 50 characters.")]
    public string Name { get; init; } = string.Empty;

    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "ColorHex must be a valid hex color code (e.g. #FF5733 or #FFF).")]
    public string ColorHex { get; init; } = "#3498db";

    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
    public string? Description { get; init; }
}

public class TagResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ColorHex { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class TagItem
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; init; }
}
