using CachingDemo.Persistance;
using CachingDemo.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.AddSerilog();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres") ?? throw new ArgumentNullException("Connection String not provided"));
});

//builder.Services.AddScoped<WeatherForcastService>();
// builder.Services.AddScoped<IWeatherForcastService, MemoryCachingWeatherForcastService>();
// builder.Services.AddMemoryCache();

builder.Services.AddScoped<WeatherForcastService>();
builder.Services.AddScoped<IWeatherForcastService, FusionMemoryCachingWeatherForcastService>();
builder.Services.AddFusionCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
