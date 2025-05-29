using Domain.Models;

namespace Domain.Services;

public interface IEventService
{
    Task<IEnumerable<Event>> GetEvents(string campus = "", string audience = "", int count = 0, Language language = Language.All);
}
