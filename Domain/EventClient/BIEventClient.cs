using Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using System.Globalization;
using System.Text.Json;

namespace Domain.EventClient;

public class BIEventClient(HttpClient client) : IBIEventClient
{
    public async Task<IEnumerable<Event>> GetEvents(string campus = "", string audience = "", int count = 0, Language language = Language.All)
    {
        var queryParams = new Dictionary<string, string>();

        if (language != Language.All)
            queryParams["language"] = language.ToString();

        if (!string.IsNullOrWhiteSpace(campus))
            queryParams["campus"] = campus;

        if (!string.IsNullOrWhiteSpace(audience))
            queryParams["audience"] = audience;

        if (count > 0)
            queryParams["take"] = count.ToString(CultureInfo.InvariantCulture);

        var query = new QueryBuilder(queryParams).ToQueryString();

        var result = await client.GetAsync($"https://www.bi.no/api/calendar-events{query}");

        result.EnsureSuccessStatusCode();

        try
        {
            var stringContent = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<IEnumerable<EventContract>>(stringContent);
            return response?.Select(e => new Event
            {
                StartTime = e.Start,
                EndTime = e.End,
                FilterList = e.FilterList,
                Id = e.Id,
                ImageText = e.ImageText,
                ImageUrl = e.ImageUrl,
                Title = e.Title,
                Location = e.Location,
                Audience = e.FilterList,
                Language = e.Language
            }) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
