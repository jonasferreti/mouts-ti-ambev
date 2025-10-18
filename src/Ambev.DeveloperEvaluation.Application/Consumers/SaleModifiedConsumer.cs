using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Consumers;

/// <summary>
/// Rebus message handler responsible for consuming the SaleModifiedEvent
/// </summary>
public class SaleModifiedConsumer : IHandleMessages<SaleModifiedEvent>
{
    private readonly ILogger<SaleCreatedConsumer> _logger;

    public SaleModifiedConsumer(ILogger<SaleCreatedConsumer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the received SaleModifiedEvent, logs the key information, and completes the message.
    /// </summary>
    /// <param name="message">The SaleModifiedEvent record containing sale details.</param>
    /// <returns>A completed task, signaling Rebus that the message was processed successfully.</returns>
    public Task Handle(SaleModifiedEvent message)
    {
        _logger.LogInformation(
            "EVENT: SaleId {SaleId} updated for Customer " +
            "{CustomerId} on {DateOccurred} with Total {TotalAmount}.",
            message.SaleId,
            message.CustomerId,
            message.DateOccurred,
            message.TotalAmount
        );

        return Task.CompletedTask;
    }
}