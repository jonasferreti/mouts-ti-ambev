namespace Ambev.DeveloperEvaluation.Common.Cache;

/// <summary>
/// Defines a contract for advanced cache management operations, specifically
/// supporting Cache Tagging/Grouping to enable mass invalidation of dependent keys
/// </summary>
public interface ICacheManager
{
    // <summary>
    /// Retrieves a cached item and automatically deserializes it to the specified type (T).
    /// </summary>
    /// <typeparam name="T">The type to deserialize the cached data into.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The deserialized object of type T, or null if the key is not found.</returns>
    Task<T?> GetItemAsync<T>(string key) where T : class;

    /// <summary>
    /// Serializes an item to JSON and saves it to the cache with an optional expiration policy.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize and cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="item">The object to cache.</param>
    /// <param name="absoluteExpirationRelativeToNow">Optional time span after which the item should expire.</param>
    Task SetItemAsync<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null);

    /// <summary>
    /// Removes a single item key from the cache. Used primarily for individual entities (e.g., a single Sale).
    /// </summary>
    /// <param name="key">The clean key (without the Redis InstanceName prefix).</param>
    Task RemoveItemAsync(string key);

    /// <summary>
    /// Associates a specific cache key (e.g., a unique list result) with a dependency tag.
    /// This key is added to a Redis Set represented by the tag name.
    /// </summary>
    /// <param name="key">The key to be grouped.</param>
    /// <param name="tag">The group name used for mass invalidation.</param>
    Task TagKeyAsync(string key, string tag);

    /// <summary>
    /// Invalidates all cache entries associated with a specific Tag by deleting all members
    /// from the dependency group (Redis Set) and then deleting the Set itself.
    /// </summary>
    /// <param name="tag">The group name.</param>
    Task InvalidateTagAsync(string tag);
}

