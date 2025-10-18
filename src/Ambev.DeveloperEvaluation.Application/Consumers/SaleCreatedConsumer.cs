using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Consumers;

/// <summary>
/// Rebus message handler responsible for consuming the SaleCreatedEvent
/// </summary>
public class SaleCreatedConsumer : IHandleMessages<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedConsumer> _logger;

    public SaleCreatedConsumer(ILogger<SaleCreatedConsumer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the received SaleCreatedEvent, logs the key information, and completes the message.
    /// </summary>
    /// <param name="message">The SaleCreatedEvent record containing sale details.</param>
    /// <returns>A completed task, signaling Rebus that the message was processed successfully.</returns>
    public Task Handle(SaleCreatedEvent message)
    {
        _logger.LogInformation(
            "EVENT: SaleId {SaleId} created for Customer " +
            "{CustomerId} on {DateOccurred} with Total {TotalAmount}.",
            message.SaleId,
            message.CustomerId,
            message.DateOccurred,
            message.TotalAmount
        );

        return Task.CompletedTask;
    }
}