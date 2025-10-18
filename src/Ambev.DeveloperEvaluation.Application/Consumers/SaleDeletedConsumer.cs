using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Consumers;

/// <summary>
/// Rebus message handler responsible for consuming the SaleDeletedEvent
/// </summary>
public class SaleDeletedConsumer : IHandleMessages<SaleDeletedEvent>
{
    private readonly ILogger<SaleDeletedConsumer> _logger;

    public SaleDeletedConsumer(ILogger<SaleDeletedConsumer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the received SaleDeletedEvent, logs the key information, and completes the message.
    /// </summary>
    /// <param name="message">The SaleDeletedEvent record containing sale details.</param>
    /// <returns>A completed task, signaling Rebus that the message was processed successfully.</returns>
    public Task Handle(SaleDeletedEvent message)
    {
        _logger.LogInformation(
            "EVENT: Sale {SaleId} was deleted on {DateOccurred}",
            message.SaleId,
            message.DateOccurred
        );

        return Task.CompletedTask;
    }
}

