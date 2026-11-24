namespace DummyLoginApi.Controllers;

using DummyLoginApi.Models;
using DummyLoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;
    private readonly ILogger<NotesController> _logger;

    public NotesController(INoteService noteService, ILogger<NotesController> logger)
    {
        _noteService = noteService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? category)
    {
        _logger.LogInformation("Retrieving notes (category: {Category}).", category);
        var notes = _noteService.GetAll(category);
        return Ok(notes);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Retrieving note {NoteId}.", id);
        var note = _noteService.GetById(id);
        if (note == null)
        {
            return NotFound(new { message = $"Note with ID '{id}' was not found." });
        }

        return Ok(note);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateNoteRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating new note with title '{Title}'.", request.Title);
        var created = _noteService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting note {NoteId}.", id);
        var deleted = _noteService.Delete(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Note with ID '{id}' was not found." });
        }

        return NoContent();
    }
}
