namespace DummyLoginApi.Models;

using System.ComponentModel.DataAnnotations;

public class CreateActivityLogRequestDto
{
    [Required(ErrorMessage = "Action is required.", AllowEmptyStrings = false)]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Action must be between 1 and 100 characters.")]
    public string Action { get; init; } = string.Empty;

    [Required(ErrorMessage = "EntityName is required.", AllowEmptyStrings = false)]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "EntityName must be between 1 and 100 characters.")]
    public string EntityName { get; init; } = string.Empty;

    public string? EntityId { get; init; }

    [StringLength(1000, ErrorMessage = "Details cannot exceed 1000 characters.")]
    public string? Details { get; init; }

    [RegularExpression("^(Info|Warning|Critical|Audit)$", ErrorMessage = "Severity must be Info, Warning, Critical, or Audit.")]
    public string Severity { get; init; } = "Info";
}

public class ActivityLogResponseDto
{
    public Guid Id { get; init; }
    public string Action { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public string? EntityId { get; init; }
    public string? Details { get; init; }
    public string Severity { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public DateTime Timestamp { get; init; }
}

public class ActivityLogItem
{
    public Guid Id { get; init; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; init; }
}
