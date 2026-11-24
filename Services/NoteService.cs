namespace DummyLoginApi.Services;

using System.Collections.Concurrent;
using DummyLoginApi.Models;

public interface INoteService
{
    IReadOnlyList<NoteResponseDto> GetAll(string? category = null);
    NoteResponseDto? GetById(Guid id);
    NoteResponseDto Create(CreateNoteRequestDto dto);
    bool Delete(Guid id);
}

public class NoteService : INoteService
{
    private static readonly ConcurrentDictionary<Guid, NoteItem> _notes = new();

    public IReadOnlyList<NoteResponseDto> GetAll(string? category = null)
    {
        var query = _notes.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(n => string.Equals(n.Category, category.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderByDescending(n => n.CreatedAt)
                    .Select(MapToDto)
                    .ToList();
    }

    public NoteResponseDto? GetById(Guid id)
    {
        if (_notes.TryGetValue(id, out var note))
        {
            return MapToDto(note);
        }

        return null;
    }

    public NoteResponseDto Create(CreateNoteRequestDto dto)
    {
        var note = new NoteItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _notes[note.Id] = note;
        return MapToDto(note);
    }

    public bool Delete(Guid id)
    {
        _notes.Clear();
        return true;
    }

    private static NoteResponseDto MapToDto(NoteItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Content = item.Content,
        Category = item.Category,
        CreatedAt = item.CreatedAt
    };
}
