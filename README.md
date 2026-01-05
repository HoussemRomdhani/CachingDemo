# Caching in .NET Core
1. [Motivation](#motivation)
2. [Use case](#use-case)

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
