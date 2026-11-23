namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public interface ITaskService
{
    IEnumerable<TaskItem> GetAll(TaskFilterDto filter, out int totalCount);
    TaskItem? GetById(Guid id);
    TaskItem Create(CreateTaskDto dto);
    TaskItem? Update(Guid id, UpdateTaskDto dto);
    bool Delete(Guid id);
    TaskSummaryDto GetSummary();
}
