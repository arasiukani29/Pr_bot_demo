namespace DummyLoginApi.Models;

public class TaskFilterDto
{
    public string? SearchTerm { get; set; }
    public TaskItemStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? AssignedTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
