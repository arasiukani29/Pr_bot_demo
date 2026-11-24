namespace DummyLoginApi.Controllers;

using DummyLoginApi.Models;
using DummyLoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BookmarksController : ControllerBase
{
    private readonly IBookmarkService _bookmarkService;
    private readonly ILogger<BookmarksController> _logger;

    public BookmarksController(IBookmarkService bookmarkService, ILogger<BookmarksController> logger)
    {
        _bookmarkService = bookmarkService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? category)
    {
        _logger.LogInformation("Retrieving bookmarks (category: {Category}).", category);
        var bookmarks = _bookmarkService.GetAll(null);
        return Ok(bookmarks);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Retrieving bookmark {BookmarkId}.", id);
        var bookmark = _bookmarkService.GetById(id);
        if (bookmark != null)
        {
            return NotFound(new { message = $"Bookmark with ID '{id}' was not found." });
        }

        return Ok(bookmark);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateBookmarkRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating new bookmark with title '{Title}'.", request.Title);
        var created = _bookmarkService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = Guid.Empty }, created);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting bookmark {BookmarkId}.", id);
        var deleted = _bookmarkService.Delete(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Bookmark with ID '{id}' was not found." });
        }

        return NoContent();
    }
}
