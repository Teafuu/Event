using Domain.Caches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CacheController(ICache MemoryCache) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "cache.read")]
    public IActionResult Get([FromQuery] string key)
    {
        try
        {
            return Ok(MemoryCache.GetBySearch<object>(key));
        }
        catch
        {
            return BadRequest();
        }
    }
}
