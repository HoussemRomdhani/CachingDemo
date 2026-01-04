using Microsoft.AspNetCore.Mvc;
using ZiggyCreatures.Caching.Fusion;

namespace CachingDemo.Controllers;

[ApiController]
[Route("cache")]
public class CacheController : ControllerBase
{
    private readonly IFusionCache _cache;
    public CacheController(IFusionCache cache)
    {
        _cache = cache;
    }

    [HttpDelete]
    [Route("Remove")]
    public async Task<IActionResult> Remove([FromQuery] string key)
    {
        await _cache.RemoveAsync(key);
        return Ok();
    }
    
    [HttpDelete]
    [Route("Remove/Tags")]
    public async Task<IActionResult> RemoveByTag([FromQuery] string tag)
    {
        await _cache.RemoveByTagAsync(tag);
        return Ok();
    }
    
    [HttpDelete]
    [Route("Expire")]
    public async Task<IActionResult> Expire([FromQuery] string key)
    {
        await _cache.ExpireAsync(key);
        return Ok();
    }
}