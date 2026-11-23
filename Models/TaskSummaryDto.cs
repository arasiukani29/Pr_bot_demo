namespace DummyLoginApi.Models;

public class TaskSummaryDto
{
    public int TotalTasks { get; set; }
    public int TodoCount { get; set; }
    public int InProgressCount { get; set; }
    public int InReviewCount { get; set; }
    public int CompletedCount { get; set; }
    public int CriticalPriorityCount { get; set; }
    public int OverdueCount { get; set; }
}
