namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public class ExpenseService : IExpenseService
{
    // In-memory store simulating a database
    private static readonly List<ExpenseDto> _expenses = new()
    {
        new ExpenseDto { Id = 1, Title = "Office Supplies", Amount = 150.00m, Category = "Office", CreatedBy = "admin", CreatedAt = DateTime.UtcNow.AddDays(-5) },
        new ExpenseDto { Id = 2, Title = "Team Lunch",      Amount = 320.50m, Category = "Food",   CreatedBy = "user",  CreatedAt = DateTime.UtcNow.AddDays(-2) },
        new ExpenseDto { Id = 3, Title = "Software License",Amount = 999.99m, Category = "Software",CreatedBy = "admin", CreatedAt = DateTime.UtcNow.AddDays(-1) },
    };

    private static int _nextId = 4;

    // ⚠️ BUG #1 — CRITICAL: SQL-injection-style filter via unsanitised string concatenation.
    // The 'search' parameter is directly embedded into the LINQ query label simulation
    // without any sanitisation. In a real DB context (EF raw SQL) this would be a full
    // SQL-injection vulnerability. Here it also causes NullReferenceException when
    // expense.Description is null because we call .Contains(search) without null-guard.
    public IEnumerable<ExpenseDto> GetAll(string? category = null, string? search = null)
    {
        var query = _expenses.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            // BUG: Description can be null → NullReferenceException at runtime
            query = query.Where(e =>
                e.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                e.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    public ExpenseDto? GetById(int id) =>
        _expenses.FirstOrDefault(e => e.Id == id);

    public ExpenseDto Create(ExpenseDto expense)
    {
        expense.Id = _nextId++;
        expense.CreatedAt = DateTime.UtcNow;
        _expenses.Add(expense);
        return expense;
    }

    public ExpenseDto? Update(int id, ExpenseDto expense)
    {
        var existing = _expenses.FirstOrDefault(e => e.Id == id);
        if (existing is null) return null;

        existing.Title       = expense.Title;
        existing.Amount      = expense.Amount;
        existing.Category    = expense.Category;
        existing.Description = expense.Description;
        return existing;
    }

    public bool Delete(int id)
    {
        var existing = _expenses.FirstOrDefault(e => e.Id == id);
        if (existing is null) return false;
        _expenses.Remove(existing);
        return true;
    }
}
