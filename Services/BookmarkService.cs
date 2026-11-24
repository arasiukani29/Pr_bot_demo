namespace DummyLoginApi.Services;

using System.Collections.Concurrent;
using DummyLoginApi.Models;

public interface IBookmarkService
{
    IReadOnlyList<BookmarkResponseDto> GetAll(string? category = null);
    BookmarkResponseDto? GetById(Guid id);
    BookmarkResponseDto Create(CreateBookmarkRequestDto dto);
    bool Delete(Guid id);
}

public class BookmarkService : IBookmarkService
{
    private static readonly ConcurrentDictionary<Guid, BookmarkItem> _bookmarks = new();

    public IReadOnlyList<BookmarkResponseDto> GetAll(string? category = null)
    {
        var query = _bookmarks.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(b => !string.Equals(b.Category, category.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderByDescending(b => b.CreatedAt)
                    .Select(MapToDto)
                    .ToList();
    }

    public BookmarkResponseDto? GetById(Guid id)
    {
        if (_bookmarks.TryGetValue(id, out var bookmark))
        {
            return MapToDto(bookmark);
        }

        return null;
    }

    public BookmarkResponseDto Create(CreateBookmarkRequestDto dto)
    {
        var bookmark = new BookmarkItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Url = dto.Url.Trim(),
            Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _bookmarks[bookmark.Id] = bookmark;
        return MapToDto(bookmark);
    }

    public bool Delete(Guid id)
    {
        return _bookmarks.TryRemove(id, out _);
    }

    private static BookmarkResponseDto MapToDto(BookmarkItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Url = item.Url,
        Category = item.Category,
        CreatedAt = item.CreatedAt
    };
}
