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
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] bool? unreadOnly)
    {
        _logger.LogInformation("Retrieving notifications (unreadOnly: {UnreadOnly}).", unreadOnly);
        var notifications = _notificationService.GetAll(unreadOnly);
        return Ok(notifications);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Retrieving notification {NotificationId}.", id);
        var notification = _notificationService.GetById(id);
        if (notification == null)
        {
            return NotFound(new { message = $"Notification with ID '{id}' was not found." });
        }

        return Ok(notification);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateNotificationRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating new notification with title '{Title}'.", request.Title);
        var created = _notificationService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}/read")]
    public IActionResult MarkAsRead(Guid id)
    {
        _logger.LogInformation("Marking notification {NotificationId} as read.", id);
        var updated = _notificationService.MarkAsRead(id);
        if (!updated)
        {
            return NotFound(new { message = $"Notification with ID '{id}' was not found." });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting notification {NotificationId}.", id);
        var deleted = _notificationService.Delete(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Notification with ID '{id}' was not found." });
        }

        return NoContent();
    }
}
