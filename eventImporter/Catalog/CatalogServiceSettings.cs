namespace eventImporter.Catalog;

public record CatalogServiceSettings(
    Uri BaseAddress,
    int RetryAttempts = 3,
    int CircuitBreakerAttempts = 5);