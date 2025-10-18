using Ambev.DeveloperEvaluation.Application.Consumers.Cache;
using Ambev.DeveloperEvaluation.Common.Cache;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Consumers;

/// <summary>
/// Rebus message handler responsible for consuming the SaleCancelledEvent
/// </summary>
public class SaleCancelledConsumer : SaleCacheInvalidatorConsumerBase<SaleCancelledEvent>
{
    private readonly ILogger<SaleCancelledConsumer> _logger;

    public SaleCancelledConsumer(
        ILogger<SaleCancelledConsumer> logger, 
        ICacheManager cacheManager) : base(cacheManager)

    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the received SaleCancelledEvent, logs the key information, and completes the message.
    /// </summary>
    /// <param name="message">The SaleCancelledEvent record containing sale details.</param>
    /// <returns>A completed task, signaling Rebus that the message was processed successfully.</returns>
    public override async Task Handle(SaleCancelledEvent message)
    {
        _logger.LogInformation(
            "EVENT: Sale {SaleId} was cancelled on {DateOccurred}",
            message.SaleId,
            message.DateOccurred
        );

        await base.Handle(message);
    }
}
