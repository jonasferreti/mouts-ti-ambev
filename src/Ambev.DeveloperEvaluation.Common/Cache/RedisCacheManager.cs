using Ambev.DeveloperEvaluation.Common.Cache;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.IoC.Cache;

/// <summary>
/// Implementation of ICacheManager using StackExchange.Redis.
/// It combines the high-level IDistributedCache for simple operations with 
/// low-level IDatabase access for managing Redis Sets (Tagging/Grouping).
/// </summary>
public class RedisCacheManager : ICacheManager
{
    private readonly IDistributedCache _distributedCache;
    private readonly IDatabase _redisDb;

    /// <summary>
    /// Initializes a new instance of the RedisCacheManager.
    /// </summary>
    /// <param name="distributedCache">Standard .NET distributed cache abstraction.</param>
    /// <param name="connectionMultiplexer">Native Redis client connection for low-level commands.</param>
    public RedisCacheManager(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
    {
        _distributedCache = distributedCache;
        _redisDb = connectionMultiplexer.GetDatabase();
    }

    /// <summary>
    /// Retrieves a cached item and automatically deserializes it to the specified type (T).
    /// </summary>
    public async Task<T?> GetItemAsync<T>(string key) where T : class
    {
        var cachedData = await _distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cachedData))
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(cachedData);
    }

    /// <summary>
    /// Serializes an item to JSON and saves it to the cache with an optional expiration policy.
    /// </summary>
    public Task SetItemAsync<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null)
    {
        var jsonData = JsonSerializer.Serialize(item);

        var options = new DistributedCacheEntryOptions();
        if (absoluteExpirationRelativeToNow.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow.Value;
        }

        return _distributedCache.SetStringAsync(key, jsonData, options);
    }

    /// <summary>
    /// Removes a single item key from the cache using IDistributedCache, 
    /// which automatically handles the 'InstanceName' prefix configured in Program.cs.
    /// </summary>
    public Task RemoveItemAsync(string key)
    {
        return _distributedCache.RemoveAsync(key);
    }

    /// <summary>
    /// Adds a given cache key to a Redis Set (the tag). This allows the key to be 
    /// found and deleted during mass invalidation.
    /// </summary>
    public async Task TagKeyAsync(string key, string tag)
    {
        var tagKey = $"{CacheConstants.SaleInstancePrefix}{tag}";

        await _redisDb.SetAddAsync(tagKey, key);
    }

    /// <summary>
    /// Performs a mass deletion of all keys associated with the provided tag.
    /// This is essential for invalidating lists when a single underlying entity changes.
    /// </summary>
    public async Task InvalidateTagAsync(string tag)
    {
        var tagKey = $"{CacheConstants.SaleInstancePrefix}{tag}";

        var keysToInvalidate = await _redisDb.SetMembersAsync(tagKey);

        if (keysToInvalidate.Length == 0) return;

        var prefixedKeysToDelete = keysToInvalidate
            .Select(key => (RedisKey)$"{CacheConstants.SaleInstancePrefix}{key}")
            .ToArray();

        await _redisDb.KeyDeleteAsync(prefixedKeysToDelete);

        await _redisDb.KeyDeleteAsync(tagKey);
    }
}
