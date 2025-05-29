using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Domain.Caches;

public class MemoryCache(ILogger<MemoryCache> Logger) : ICache
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

    public Task<T?> GetAsync<T>(string key)
    {
        if (!_cache.TryGetValue(key, out var entry))
            return Task.FromResult<T?>(default);

        if (entry.Expiration is not null && entry.Expiration < DateTimeOffset.UtcNow)
        {
            _cache.TryRemove(key, out _);
            Logger.LogDebug("Cache expired for key: {Key}", key);
            return Task.FromResult<T?>(default);
        }

        return Task.FromResult((T?)entry.Value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        var entry = new CacheEntry
        {
            Value = value!,
            Expiration = absoluteExpiration.HasValue
                ? DateTimeOffset.UtcNow.Add(absoluteExpiration.Value)
                : null
        };

        Logger.LogDebug($"Cache entry for key: {key}");

        _cache.AddOrUpdate(key, entry, (_, __) => entry);
        return Task.CompletedTask;
    }

    public Task<T?> GetBySearch<T>(string key) => Task.FromResult((T?)_cache.FirstOrDefault(x => x.Key.Contains(key)).Value.Value);

    private class CacheEntry
    {
        public object? Value { get; set; } = null;
        public DateTimeOffset? Expiration { get; set; }
    }
}
