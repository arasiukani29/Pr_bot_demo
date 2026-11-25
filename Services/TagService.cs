namespace DummyLoginApi.Services;

using System.Collections.Concurrent;
using DummyLoginApi.Models;

public interface ITagService
{
    IReadOnlyList<TagResponseDto> GetAll(string? search = null);
    TagResponseDto? GetById(Guid id);
    TagResponseDto Create(CreateTagRequestDto dto);
    bool Delete(Guid id);
}

public class TagService : ITagService
{
    private static readonly ConcurrentDictionary<Guid, TagItem> _tags = new();

    static TagService()
    {
        var initial = new[]
        {
            new TagItem { Id = Guid.NewGuid(), Name = "Urgent", ColorHex = "#e74c3c", Description = "High priority items", CreatedAt = DateTime.UtcNow },
            new TagItem { Id = Guid.NewGuid(), Name = "Work", ColorHex = "#2ecc71", Description = "Professional tasks and resources", CreatedAt = DateTime.UtcNow },
            new TagItem { Id = Guid.NewGuid(), Name = "Personal", ColorHex = "#9b59b6", Description = "Personal bookmarks and notes", CreatedAt = DateTime.UtcNow }
        };

        foreach (var tag in initial)
        {
            _tags[tag.Id] = tag;
        }
    }

    public IReadOnlyList<TagResponseDto> GetAll(string? search = null)
    {
        var query = _tags.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(t => t.Name)
                    .Select(MapToDto)
                    .ToList();
    }

    public TagResponseDto? GetById(Guid id)
    {
        if (_tags.TryGetValue(id, out var tag))
        {
            return MapToDto(tag);
        }

        return null;
    }

    public TagResponseDto Create(CreateTagRequestDto dto)
    {
        var tag = new TagItem
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            ColorHex = string.IsNullOrWhiteSpace(dto.ColorHex) ? "#3498db" : dto.ColorHex.Trim(),
            Description = dto.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _tags[tag.Id] = tag;
        return MapToDto(tag);
    }

    public bool Delete(Guid id)
    {
        return _tags.TryRemove(id, out _);
    }

    private static TagResponseDto MapToDto(TagItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        ColorHex = item.ColorHex,
        Description = item.Description,
        CreatedAt = item.CreatedAt
    };
}
