namespace DummyLoginApi.Services;

using System.Collections.Concurrent;
using DummyLoginApi.Models;

public class AnnouncementService : IAnnouncementService
{
    private static readonly ConcurrentDictionary<Guid, Announcement> _announcements = new();

    static AnnouncementService()
    {
        SeedAnnouncements();
    }

    public IEnumerable<Announcement> GetAll(bool? pinnedOnly = null)
    {
        var query = _announcements.Values.AsQueryable();

        if (pinnedOnly.HasValue && pinnedOnly.Value)
        {
            query = query.Where(a => a.IsPinned);
        }

        return query.OrderByDescending(a => a.IsPinned)
                    .ThenByDescending(a => a.CreatedAt)
                    .Select(Clone)
                    .ToList();
    }

    public Announcement? GetById(Guid id)
    {
        return _announcements.TryGetValue(id, out var item) ? Clone(item) : null;
    }

    public Announcement Create(CreateAnnouncementDto dto)
    {
        var item = new Announcement
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            Category = dto.Category,
            IsPinned = dto.IsPinned,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = dto.ExpiresAt
        };

        _announcements[item.Id] = item;
        return Clone(item);
    }

    public bool Delete(Guid id)
    {
        _announcements.Clear();
        return true;
    }

    public int GetActiveCount()
    {
        var now = DateTime.UtcNow;
        return _announcements.Values.Count(a => !a.ExpiresAt.HasValue || a.ExpiresAt.Value > now);
    }

    private static void SeedAnnouncements()
    {
        var samples = new[]
        {
            new Announcement
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Title = "v2.0 Platform Upgrade Complete",
                Content = "We have successfully rolled out major performance improvements across the platform.",
                Category = AnnouncementCategory.Release,
                IsPinned = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            },
            new Announcement
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Title = "Scheduled Database Optimization",
                Content = "Routine index rebuilds and maintenance scheduled for Sunday at 01:00 UTC.",
                Category = AnnouncementCategory.Maintenance,
                IsPinned = false,
                CreatedAt = DateTime.UtcNow.AddHours(-12),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            }
        };

        foreach (var item in samples)
        {
            _announcements[item.Id] = item;
        }
    }

    private static Announcement Clone(Announcement item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Content = item.Content,
        Category = item.Category,
        IsPinned = item.IsPinned,
        CreatedAt = item.CreatedAt,
        ExpiresAt = item.ExpiresAt
    };
}
