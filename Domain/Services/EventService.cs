using Domain.Caches;
using Domain.EventClient;
using Domain.Models;

namespace Domain.Services;

public class EventService(IBIEventClient BIClient, ICache cache) : IEventService
{
    public async Task<IEnumerable<Event>> GetEvents(
        string campus = "",
        string audience = "",
        int count = 0,
        Language language = Language.All)
    {
        var cacheKey = GenerateCacheKey(campus, audience, count, language);

        var cachedEvents = await cache.GetAsync<IEnumerable<Event>>(cacheKey);
        if (cachedEvents is not null)
            return cachedEvents;

        var newEvents = await BIClient.GetEvents(campus, audience, count, language);
        await cache.SetAsync(cacheKey, newEvents);

        return newEvents;
    }

    private string GenerateCacheKey(string campus = "", string audience = "", int count = 0, Language language = Language.All)
        => $"calendar-events:{campus}:{audience}:{count}:{language.ToString().ToLowerInvariant()}";
}
