using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using System;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;

/// <summary>
/// Provides test objects for the DeleteSaleCommand.
/// </summary>
public static class DeleteSaleHandlerTestData
{
    private static readonly Guid ValidSaleId = Guid.NewGuid();
    private static readonly Guid AnotherValidSaleId = Guid.NewGuid();

    /// <summary>
    /// Generates a valid DeleteSaleCommand with a pre-defined SaleId.
    /// </summary>
    public static DeleteSaleCommand GenerateValidCommand()
    {
        return new DeleteSaleCommand(ValidSaleId);
    }

    /// <summary>
    /// Generates a DeleteSaleCommand with a different SaleId for Not Found scenarios.
    /// </summary>
    public static DeleteSaleCommand GenerateNotFoundCommand()
    {
        return new DeleteSaleCommand(AnotherValidSaleId);
    }

    /// <summary>
    /// Generates an invalid command with an empty SaleId (violates NotEmpty rule).
    /// </summary>
    public static DeleteSaleCommand GenerateInvalidCommand_EmptySaleId()
    {
        return new DeleteSaleCommand(Guid.Empty);
    }

    /// <summary>
    /// Provides the Sale ID used for the valid command (for use in the repository arrangement).
    /// </summary>
    public static Guid ValidIdForArrangement() => ValidSaleId;
}