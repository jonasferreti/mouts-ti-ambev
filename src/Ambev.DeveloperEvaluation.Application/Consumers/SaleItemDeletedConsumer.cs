using Ambev.DeveloperEvaluation.Application.Consumers.Cache;
using Ambev.DeveloperEvaluation.Common.Cache;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Consumers;


/// <summary>
/// Rebus message handler responsible for consuming the SaleItemDeletedEvent
/// </summary>
public class SaleItemDeletedConsumer : SaleCacheInvalidatorConsumerBase<SaleItemDeletedEvent>
{
    private readonly ILogger<SaleItemDeletedConsumer> _logger;

    public SaleItemDeletedConsumer(
        ILogger<SaleItemDeletedConsumer> logger,
        ICacheManager cacheManager) : base(cacheManager)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the received SaleItemDeletedEvent, logs the key information, and completes the message.
    /// </summary>
    /// <param name="message">The SaleItemDeletedEvent record containing sale details.</param>
    /// <returns>A completed task, signaling Rebus that the message was processed successfully.</returns>
    public override async Task Handle(SaleItemDeletedEvent message)
    {
        _logger.LogInformation(
            "EVENT: SaleItem {ItemId} for Sale {SaleId} was deleted on {DateOccurred}",
            message.ItemId,
            message.SaleId,
            message.DateOccurred
        );

        await base.Handle(message);
    }
}
