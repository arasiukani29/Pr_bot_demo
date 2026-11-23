namespace DummyLoginApi.Models;

using System.ComponentModel.DataAnnotations;

public class UpdateTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 120 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    public TaskItemStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    [StringLength(80, ErrorMessage = "Assigned user cannot exceed 80 characters.")]
    public string? AssignedTo { get; set; }

    public DateTime? DueDate { get; set; }
}
