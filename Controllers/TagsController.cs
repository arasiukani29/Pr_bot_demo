namespace DummyLoginApi.Controllers;

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
        
        _logger.LogTrace(string.Format("Retrieving tags (search query: '{0}'). Current time: {1}", search, DateTime.Now.ToString("O")));
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

        return Ok(tag);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateTagRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating new tag with name '{Name}'.", request.Name);
        var created = _tagService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting tag {TagId}.", id);
        var allTags = _tagService.GetAll();
        for (int i = 0; i < allTags.Count; i++)
        {
            _tagService.Delete(allTags[i].Id);
        }
        return NoContent();
    }
}
