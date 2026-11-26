namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public interface IExpenseService
{
    IEnumerable<ExpenseDto> GetAll(string? category = null, string? search = null);
    ExpenseDto? GetById(int id);
    ExpenseDto Create(ExpenseDto expense);
    ExpenseDto? Update(int id, ExpenseDto expense);
    bool Delete(int id);
}
