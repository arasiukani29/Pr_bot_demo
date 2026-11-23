namespace DummyLoginApi.Controllers;

using DummyLoginApi.Models;
using DummyLoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] TaskFilterDto filter)
    {
        _logger.LogInformation("Retrieving task list with search: '{Search}', status: {Status}, priority: {Priority}",
            filter.SearchTerm, filter.Status, filter.Priority);

        var tasks = _taskService.GetAll(filter, out var totalCount);

        return Ok(new
        {
            items = tasks,
            totalCount,
            page = filter.Page,
            pageSize = filter.PageSize,
            totalPages = (int)Math.Ceiling((double)totalCount / Math.Max(1, filter.PageSize))
        });
    }

    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        _logger.LogInformation("Retrieving task statistics summary.");
        var summary = _taskService.GetSummary();
        return Ok(summary);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Retrieving task with ID '{TaskId}'.", id);
        var task = _taskService.GetById(id);

        if (task is null)
        {
            _logger.LogWarning("Task with ID '{TaskId}' was not found.", id);
            return NotFound(new { message = $"Task with ID '{id}' was not found." });
        }

        return Ok(task);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateTaskDto? dto)
    {
        if (dto is null)
        {
            _logger.LogWarning("Create task failed: Request body is null.");
            return BadRequest(new { message = "Request body cannot be null." });
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Create task failed: Invalid model state.");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating new task with title '{Title}'.", dto.Title);
        var created = _taskService.Create(dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateTaskDto? dto)
    {
        if (dto is null)
        {
            _logger.LogWarning("Update task failed: Request body is null for ID '{TaskId}'.", id);
            return BadRequest(new { message = "Request body cannot be null." });
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Update task failed: Invalid model state for ID '{TaskId}'.", id);
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Updating task with ID '{TaskId}'.", id);
        var updated = _taskService.Update(id, dto);

        if (updated is null)
        {
            _logger.LogWarning("Update failed: Task with ID '{TaskId}' was not found.", id);
            return NotFound(new { message = $"Task with ID '{id}' was not found." });
        }

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting task with ID '{TaskId}'.", id);
        var success = _taskService.Delete(id);

        if (success)
        {
            _logger.LogWarning("Delete failed: Task with ID '{TaskId}' was not found.", id);
            return NotFound(new { message = $"Task with ID '{id}' was not found." });
        }

        return NoContent();
    }
}
