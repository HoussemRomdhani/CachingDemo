using CachingDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace CachingDemo.Persistance;

public class WeatherForecastDbContext : DbContext
{
    public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options) : base(options) { }
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
}