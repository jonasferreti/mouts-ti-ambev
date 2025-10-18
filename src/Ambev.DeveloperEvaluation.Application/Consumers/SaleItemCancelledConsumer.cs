using Ambev.DeveloperEvaluation.Application.Consumers.Cache;
using Ambev.DeveloperEvaluation.Common.Cache;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Consumers;


/// <summary>
/// Rebus message handler responsible for consuming the SaleItemCancelledEvent
/// </summary>
public class SaleItemCancelledConsumer : SaleCacheInvalidatorConsumerBase<SaleItemCancelledEvent>
{
    private readonly ILogger<SaleItemCancelledConsumer> _logger;

    public SaleItemCancelledConsumer(
        ILogger<SaleItemCancelledConsumer> logger,
        ICacheManager cacheManager) : base(cacheManager)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the received SaleItemCancelledEvent, logs the key information, and completes the message.
    /// </summary>
    /// <param name="message">The SaleItemCancelledEvent record containing sale details.</param>
    /// <returns>A completed task, signaling Rebus that the message was processed successfully.</returns>
    public override async Task Handle(SaleItemCancelledEvent message)
    {
        _logger.LogInformation(
            "EVENT: SaleItem {ItemId} for Sale {SaleId} was cancelled on {DateOccurred}",
            message.ItemId,
            message.SaleId,
            message.DateOccurred
        );

        await base.Handle(message);
    }
}
