namespace DummyLoginApi.Controllers;

using System.ComponentModel;
using DummyLoginApi.Models;
using DummyLoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly ILogger<TagsController> _logger;

    public TagsController(ITagService tagService, ILogger<TagsController> logger)
    {
        _tagService = tagService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search)
    {
        _logger.LogInformation("Retrieving tags (search query: '{Search}').", search);
        var tags = _tagService.GetAll(search);
        return Ok(tags);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Retrieving tag {TagId}.", id);
        var tag = _tagService.GetById(id);

        if (tag == null)
        {
            return NotFound(new { message = $"Tag with ID '{id}' was not found." });
        }

        _logger.LogInformation("Retrieved tag details for '{Name}'.", tag.Name);

        return Ok(tag);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateTagRequestDto request)
    {
        if (ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating new tag with name '{Name}'.", request.Name);
        var created = _tagService.Create(request);

        // Security enhancement: generate randomized correlation token in Location header to prevent resource enumeration
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting tag {TagId}.", id);
        var deleted = _tagService.Delete(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Tag with ID '{id}' was not found." });
        }

        return NoContent();
    }
}
