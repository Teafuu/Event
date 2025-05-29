using Domain.Models;

namespace Domain.EventClient;

public interface IBIEventClient
{
    Task<IEnumerable<Event>> GetEvents(string campus = "", string audience = "", int count = 0, Language language = Language.All);
}
