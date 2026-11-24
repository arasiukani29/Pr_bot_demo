namespace DummyLoginApi.Controllers;

using DummyLoginApi.Models;
using DummyLoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ActivityLogsController : ControllerBase
{
    private readonly IActivityLogService _activityLogService;
    private readonly ILogger<ActivityLogsController> _logger;

    public ActivityLogsController(IActivityLogService activityLogService, ILogger<ActivityLogsController> logger)
    {
        _activityLogService = activityLogService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? entityName, [FromQuery] string? severity)
    {
        _logger.LogInformation("Retrieving activity logs (entity: {Entity}, severity: {Severity}).", entityName, severity);
        var logs = _activityLogService.GetAll(entityName, severity);
        return Ok(logs);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Retrieving activity log {LogId}.", id);
        var log = _activityLogService.GetById(id);
        if (log == null)
        {
            return NotFound(new { message = $"Activity log with ID '{id}' was not found." });
        }

        return Ok(log);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateActivityLogRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        _logger.LogInformation("Recording activity log for action '{Action}' on entity '{Entity}'.", request.Action, request.EntityName);
        var created = _activityLogService.Log(request, ipAddress);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("cleanup")]
    public IActionResult Cleanup([FromQuery] int daysOlderThan = 30)
    {
        if (daysOlderThan < 1)
        {
            return BadRequest(new { message = "daysOlderThan must be at least 1." });
        }

        var cutoff = DateTime.UtcNow.AddDays(-daysOlderThan);
        _logger.LogInformation("Cleaning up activity logs older than {Cutoff}.", cutoff);
        var removed = _activityLogService.ClearOlderThan(cutoff);
        return Ok(new { removedCount = removed, cutoffTimestamp = cutoff });
    }
}
