using CachingDemo.Persistance;
using CachingDemo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Serilog;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

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

// builder.Services.AddScoped<WeatherForcastService>();
// builder.Services.AddScoped<IWeatherForcastService, FusionMemoryCachingWeatherForcastService>();
// builder.Services.AddFusionCache();

/*
builder.Services.AddScoped<WeatherForcastService>();
builder.Services.AddScoped<IWeatherForcastService, DistributedCachingWeatherForcastService>();
builder.Services.AddFusionCache()
        .WithDefaultEntryOptions(new FusionCacheEntryOptions {
            SkipMemoryCacheRead = true,
            SkipMemoryCacheWrite = true,
        })
    .WithSerializer(new FusionCacheSystemTextJsonSerializer())
    .WithDistributedCache(new RedisCache(new RedisCacheOptions()
    {
        Configuration = builder.Configuration.GetConnectionString("Redis")
    }));*/

builder.Services.AddScoped<WeatherForcastService>();
builder.Services.AddScoped<IWeatherForcastService, HybridCachingWeatherForcastService>();
builder.Services.AddFusionCache()
    .WithDefaultEntryOptions(new FusionCacheEntryOptions
    {
        Duration = TimeSpan.MaxValue,
    })
    .WithSerializer(new FusionCacheSystemTextJsonSerializer())
    .WithDistributedCache(new RedisCache(new RedisCacheOptions()
    {
        Configuration = builder.Configuration.GetConnectionString("Redis"),
    }))
    .WithBackplane(
        new RedisBackplane(new RedisBackplaneOptions
        {
            Configuration = builder.Configuration.GetConnectionString("Redis"),
        })
    )
    .AsHybridCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

var cache = app.Services.GetRequiredService<IFusionCache>();
cache.Events.Memory.Hit += (sender, args) =>
{
    if (!args.Key.StartsWith("__fc"))
    {
      Console.WriteLine($"[Memory Cache] Hit key: {args.Key}");
    }
};
cache.Events.Memory.Miss += (sender, args) =>
{
    if (!args.Key.StartsWith("__fc"))
    {
       Console.WriteLine($"[Memory Cache] Miss key: {args.Key}");
    }
};

cache.Events.Distributed.Hit += (sender, args) =>
{
    if (!args.Key.StartsWith("__fc"))
    {
        Console.WriteLine($"[Distributed Cache] Hit key: {args.Key}");
    }
};

cache.Events.Distributed.Miss += (sender, args) =>
{
    if (!args.Key.StartsWith("__fc"))
    {
      Console.WriteLine($"[Distributed Cache] Miss key: {args.Key}");
    }
};
app.Run();
