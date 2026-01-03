using CachingDemo.Models;

namespace CachingDemo.Services;

public interface IWeatherForcastService
{
    Task<WeatherForecast?> Get(string city);
}