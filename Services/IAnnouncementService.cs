namespace DummyLoginApi.Services;

using DummyLoginApi.Models;

public interface IAnnouncementService
{
    IEnumerable<Announcement> GetAll(bool? pinnedOnly = null);
    Announcement? GetById(Guid id);
    Announcement Create(CreateAnnouncementDto dto);
    bool Delete(Guid id);
    int GetActiveCount();
}
