using CachingDemo.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CachingDemo.Services;

public class MemoryCachingWeatherForcastService : IWeatherForcastService
{
    private readonly WeatherForcastService _service;
    private readonly IMemoryCache _cache;
    public MemoryCachingWeatherForcastService(WeatherForcastService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }

    public async Task<WeatherForecast?> Get(string city)
    {
        return await _cache.GetOrCreateAsync<WeatherForecast?>(
            $"weatherforecast_{city}",
            async (entry) => await _service.Get(city));
    }
}