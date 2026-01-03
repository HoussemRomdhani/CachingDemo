using Microsoft.AspNetCore.Mvc;
using CachingDemo.Persistance;
using CachingDemo.Services;

namespace CachingDemo.Controllers;

[ApiController]
[Route("weatherForecast")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForcastService _service;
    public WeatherForecastController(IWeatherForcastService service)
    {
        _service = service;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get([FromQuery] string city)
    {
        var forecast = await _service.Get(city);
        return forecast != null ? Ok(forecast) : NotFound();
    }
}