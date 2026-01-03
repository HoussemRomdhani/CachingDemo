using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CachingDemo.Controllers;

[ApiController]
[Route("cache")]
public class CacheController : ControllerBase
{
    private readonly IMemoryCache _cache;
    public CacheController(IMemoryCache cache)
    {
        _cache = cache;
    }

    [HttpDelete]
    public IActionResult Remove([FromQuery] string key)
    {
        _cache.Remove(key);
        return Ok();
    }
}