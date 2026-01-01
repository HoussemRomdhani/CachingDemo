using System.ComponentModel.DataAnnotations;

namespace CachingDemo.Models;

public class WeatherForecast
{
    [Key]
    public string City { get; set; } = string.Empty;
    public int TemperatureC { get; set; }
}