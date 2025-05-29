using Api.Models;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController(ILogger<EventsController> Logger, IEventService EventService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> Get(
        [FromQuery] string campus = "",
        [FromQuery] string audience = "",
        [FromQuery] int count = 0,
        [FromQuery] Language language = Language.All)
    {
        try
        {
            var result = await EventService.GetEvents(
                campus: campus,
                audience: audience,
                count: count,
                language: language);

            return Ok(result.Select(e => new EventResponse
            {
                EndTime = e.EndTime,
                Audience = e.Audience,
                Id = e.Id,
                Language = e.Language,
                ImageText = e.ImageText,
                ImageUrl = e.ImageUrl,
                Location = e.Location,
                StartTime = e.StartTime,
                Title = e.Title,
            }));
        } 
        catch(Exception e)
        {
            Logger.LogError($"Failed to get events: {e}");
        }

        return Problem($"Failed to get events");
    }
}
