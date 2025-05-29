namespace Domain.Caches;

public interface ICache
{
    Task<T?> GetAsync<T>(string key);

    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null);

    Task<T?> GetBySearch<T>(string key);
}
