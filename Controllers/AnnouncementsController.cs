namespace DummyLoginApi.Controllers;

using DummyLoginApi.Models;
using DummyLoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;
    private readonly ILogger<AnnouncementsController> _logger;

    public AnnouncementsController(IAnnouncementService announcementService, ILogger<AnnouncementsController> logger)
    {
        _announcementService = announcementService ?? throw new ArgumentNullException(nameof(announcementService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] bool? pinnedOnly)
    {
        _logger.LogInformation("Retrieving announcements (pinnedOnly: {PinnedOnly}).", pinnedOnly);
        var items = _announcementService.GetAll(pinnedOnly);
        return Ok(items);
    }

    [HttpGet("active-count")]
    public IActionResult GetActiveCount()
    {
        var count = _announcementService.GetActiveCount();
        return Ok(new { activeCount = count });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Retrieving announcement with ID '{AnnouncementId}'.", id);
        var item = _announcementService.GetById(id);

        if (item is null)
        {
            _logger.LogWarning("Announcement '{AnnouncementId}' not found.", id);
            return NotFound(new { message = $"Announcement '{id}' was not found." });
        }

        return Ok(item);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateAnnouncementDto? dto)
    {
        if (dto is null)
        {
            _logger.LogWarning("Create announcement failed: Request body is null.");
            return BadRequest(new { message = "Request body cannot be null." });
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            ModelState.AddModelError(nameof(dto.Title), "Title cannot be empty or contain only whitespace.");
        }

        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            ModelState.AddModelError(nameof(dto.Content), "Content cannot be empty or contain only whitespace.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating announcement '{Title}'.", dto.Title);
        var created = _announcementService.Create(dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting announcement with ID '{AnnouncementId}'.", id);
        var success = _announcementService.Delete(id);

        if (!success)
        {
            _logger.LogWarning("Delete failed: Announcement '{AnnouncementId}' not found.", id);
            return NotFound(new { message = $"Announcement '{id}' was not found." });
        }

        return NoContent();
    }
}
