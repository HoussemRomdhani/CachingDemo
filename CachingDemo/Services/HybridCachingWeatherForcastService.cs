using CachingDemo.Models;
using ZiggyCreatures.Caching.Fusion;

namespace CachingDemo.Services;

public class HybridCachingWeatherForcastService : IWeatherForcastService
{
    private readonly WeatherForcastService _service;
    private readonly IFusionCache _cache;
    public HybridCachingWeatherForcastService(WeatherForcastService service, IFusionCache cache)
    {
        _service = service;
        _cache = cache;
    }

    public async Task<WeatherForecast?> Get(string city)
    {
        return await _cache.GetOrSetAsync<WeatherForecast?>(
            $"weatherforecast_{city}",
            async (entry) => await _service.Get(city),
            tags: new[] {"weatherforecast"});
    }
}