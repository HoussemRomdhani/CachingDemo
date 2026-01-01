using CachingDemo.Models;
using CachingDemo.Persistance;
using Microsoft.EntityFrameworkCore;

namespace CachingDemo.Services;

public class WeatherForcastService
{
    private readonly WeatherForecastDbContext _context;
    public WeatherForcastService(WeatherForecastDbContext context)
    {
        _context = context;
    }

    public async Task<WeatherForecast?> Get(string city)
    {
        return await _context.WeatherForecasts.FirstOrDefaultAsync(w => w.City == city);
    }
}
