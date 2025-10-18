using Ambev.DeveloperEvaluation.Common.Cache;
using Ambev.DeveloperEvaluation.Domain.Events;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Consumers.Cache;

/// <summary>
/// Abstract base consumer for all events that require the invalidation of a Sale cache entry.
/// It implements the core logic to remove the cache key from Redis.
/// </summary>
/// <typeparam name="TEvent">The specific event type that implements IDomainEvent.</typeparam>
public abstract class SaleCacheInvalidatorConsumerBase<TEvent> : IHandleMessages<TEvent>
    where TEvent : IDomainEvent
{
    private readonly ICacheManager _cacheManager;

    protected SaleCacheInvalidatorConsumerBase(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    /// <summary>
    /// Handles the received event by asynchronously removing the corresponding Sale key from the distributed cache (Redis).
    /// </summary>
    /// <param name="message">The event message containing the SaleId.</param>
    public virtual async Task Handle(TEvent message)
    {
        await _cacheManager.RemoveItemAsync(message.SaleId.ToString());
        await _cacheManager.InvalidateTagAsync(CacheConstants.SalesListTag);
    }
}
