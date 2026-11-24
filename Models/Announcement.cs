namespace DummyLoginApi.Models;

public enum AnnouncementCategory
{
    General,
    Release,
    Maintenance,
    Security
}

public class Announcement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementCategory Category { get; set; } = AnnouncementCategory.General;
    public bool IsPinned { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}
