namespace DummyLoginApi.Controllers;

using DummyLoginApi.Models;
using DummyLoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] bool? unreadOnly)
    {
        _logger.LogInformation("Retrieving notifications (unreadOnly: {UnreadOnly}).", unreadOnly);
        var items = _notificationService.GetAll(unreadOnly);
        return Ok(items);
    }

    [HttpGet("unread-count")]
    public IActionResult GetUnreadCount()
    {
        var count = _notificationService.GetUnreadCount();
        return Ok(new { unreadCount = count });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var item = _notificationService.GetById(id);
        if (item is null)
        {
            _logger.LogWarning("Notification '{NotificationId}' not found.", id);
            return NotFound(new { message = $"Notification '{id}' was not found." });
        }

        return Ok(item);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateNotificationDto? dto)
    {
        if (dto is null)
        {
            return BadRequest(new { message = "Request body cannot be null." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = _notificationService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/read")]
    public IActionResult MarkAsRead(Guid id)
    {
        var success = _notificationService.MarkAsRead(id);
        if (!success)
        {
            return NotFound(new { message = $"Notification '{id}' was not found." });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var success = _notificationService.Delete(id);
        if (!success)
        {
            return NotFound(new { message = $"Notification '{id}' was not found." });
        }

        return NoContent();
    }
}
