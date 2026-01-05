# Caching in .NET Core
1. [Motivation](#motivation)
2. [Use case](#use-case)
3. [Cache en mémoire de Microsoft et ses limitations](#cache-en-mémoire-de-microsoft-et-ses-limitations)
4. [FusionCache](#fusioncache)
5. [Cache distribué](#cache-distribué)

## Motivation
- **Performance** : réduction de la latence (accès mémoire ou base de données destinée au cache (Redis) vs base de données métier)
- **Réduction de charge** : moins de requêtes vers la base de données métier
- **Scalabilité** : capacité à gérer plus de requêtes avec les mêmes ressources
- **Expérience utilisateur** : réponses plus rapides pour les utilisateurs
- **Résilience** : protection contre les surcharges de la base de données

## Use case
- Données fréquemment consultées mais rarement modifiées
- Résultats de calculs coûteux
- Données provenant d'APIs externes lentes
- Configuration d'application

## Cache en Mémoire de Microsoft et ses limitations
### Implémentation
```csharp
// Configuration
builder.Services.AddMemoryCache();

// Utilisation
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
```
### Limitations
#### Problème 1 : Cache Stampede 
   ```
Temps T0 : Cache expire ou vide pour "weatherforecact_Paris" ❌

Temps T1 : 100 requêtes simultanées arrivent

Requête 1 ──┐
Requête 2 ──┤
Requête 3 ──┤
    ...     ├──> Cache vide/expiré → Toutes vont à la DB
Requête 98 ─┤
Requête 99 ─┤
Requête 100┘

            ↓
    ┌─────────────────┐
    │  Base de Données │
    │                  │
    │  💥 100 requêtes  │ ← SURCHARGE !
    │     simultanées  │
    │                  │
    └─────────────────┘
            │
            ↓
    Timeouts, Erreurs, Dégradation
```
**Pas de protection (IMemoryCache) :**
- 100 requêtes → 100 appels DB simultanés
- Surcharge de la base de données
- Risque de panne en cascade
- 
#### Problème 2 : Absence de Fail-Safe
```
Temps T0 : Cache contient "weatherforecact_Paris" (valide jusqu'à T1)
           Cache: weatherforecact_Paris = "Valeur X" ✅ (expire à T1)

Temps T1 : Cache expire
           Cache: weatherforecact_Paris = EXPIRÉ ❌

Temps T2 : Base de données tombe en panne 💥
           DB: INDISPONIBLE ❌

Temps T3 : Requête arrive pour "weatherforecact_Paris"

Requête ──> Cache vide/expiré
         │
         └──> Tentative DB
              │
              └──> 💥 ERREUR (DB en panne)
                   │
                   └──> ❌ Retourne null/erreur
                        Application indisponible
```
**Pas de protection (IMemoryCache) :**
- Cache expiré/vide + DB en panne = Application indisponible
- Pas de valeur de secours disponible
- Toutes les requêtes échouent

## FusionCache
![img.png](img.png)
### Features
- **Fail-Safe** : protection automatique contre les pannes
- **Anti-Stampede** : évite les requêtes multiples simultanées
- **Factory Pattern** : pattern de factory pour la récupération des données
- **Support des Tags** : invalidation par tags
### Implémentation
```csharp
// Configuration
builder.Services.AddFusionCache();

// Utilisation
public class FusionMemoryCachingWeatherForcastService : IWeatherForcastService
{
    private readonly WeatherForcastService _service;
    private readonly IFusionCache _cache;
    public FusionMemoryCachingWeatherForcastService(WeatherForcastService service, IFusionCache cache)
    {
        _service = service;
        _cache = cache;
    }

    public async Task<WeatherForecast?> Get(string city)
    {
        return await _cache.GetOrSetAsync<WeatherForecast?>(
            $"weatherforecast_{city}",
            async (entry) => await _service.Get(city));
    }
}
```

## Cache distribué

### Implémentation

```csharp
// Configuration
builder.Services.AddFusionCache()
        .WithDefaultEntryOptions(new FusionCacheEntryOptions {
            SkipMemoryCacheRead = true,
            SkipMemoryCacheWrite = true,
        })
    .WithSerializer(new FusionCacheSystemTextJsonSerializer())
    .WithDistributedCache(new RedisCache(new RedisCacheOptions()
    {
        Configuration = builder.Configuration.GetConnectionString("Redis")
    }));

// Utilisation
public class DistributedCachingWeatherForcastService : IWeatherForcastService
{
    private readonly WeatherForcastService _service;
    private readonly IFusionCache _cache;
    public DistributedCachingWeatherForcastService(WeatherForcastService service, IFusionCache cache)
    {
        _service = service;
        _cache = cache;
    }

    public async Task<WeatherForecast?> Get(string city)
    {
        return await _cache.GetOrSetAsync<WeatherForecast?>(
            $"weatherforecast_{city}",
            async (entry) => await _service.Get(city));
    }
}
```