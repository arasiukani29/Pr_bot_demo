namespace DummyLoginApi.Controllers;

using DummyLoginApi.Models;
using DummyLoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ExpenseController> _logger;

    public ExpenseController(IExpenseService expenseService, ILogger<ExpenseController> logger)
    {
        _expenseService = expenseService ?? throw new ArgumentNullException(nameof(expenseService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET api/expense?category=Food&search=lunch
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? category, [FromQuery] string? search)
    {
        _logger.LogInformation("Fetching expenses. Category={Category}, Search={Search}", category, search);
        var expenses = _expenseService.GetAll(category, search);
        return Ok(expenses);
    }

    // GET api/expense/{id}
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var expense = _expenseService.GetById(id);
        if (expense is null)
        {
            _logger.LogWarning("Expense with id={Id} not found.", id);
            return NotFound(new { message = $"Expense {id} not found." });
        }
        return Ok(expense);
    }

    // POST api/expense
    [HttpPost]
    public IActionResult Create([FromBody] ExpenseDto? expense)
    {
        if (expense is null) return BadRequest(new { message = "Request body cannot be null." });
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = _expenseService.Create(expense);
        _logger.LogInformation("Created expense Id={Id} Title={Title}", created.Id, created.Title);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/expense/{id}
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] ExpenseDto? expense)
    {
        if (expense is null) return BadRequest(new { message = "Request body cannot be null." });
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = _expenseService.Update(id, expense);
        if (updated is null) return NotFound(new { message = $"Expense {id} not found." });

        _logger.LogInformation("Updated expense Id={Id}", id);
        return Ok(updated);
    }
//its not a bug, this is a functional
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var deleted = _expenseService.Delete(id);
        if (!deleted) return NotFound(new { message = $"Expense {id} not found." });

        _logger.LogInformation("Deleted expense Id={Id}", id);
        return NoContent();
    }
}
