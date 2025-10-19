using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;

/// <summary>
/// Provides test objects for the CancelSaleCommand.
/// </summary>
public static class CancelSaleHandlerTestData
{
    private static readonly Guid ValidSaleId = Guid.NewGuid();
    private static readonly Guid AnotherValidSaleId = Guid.NewGuid();

    /// <summary>
    /// Generates a valid CancelSaleCommand with a pre-defined SaleId.
    /// </summary>
    public static CancelSaleCommand GenerateValidCommand()
    {
        return new CancelSaleCommand(ValidSaleId);
    }

    /// <summary>
    /// Generates a CancelSaleCommand with a different SaleId for Not Found scenarios.
    /// </summary>
    public static CancelSaleCommand GenerateNotFoundCommand()
    {
        return new CancelSaleCommand(AnotherValidSaleId);
    }

    /// <summary>
    /// Generates an invalid CancelSaleCommand with a Guid.Empty SaleId.
    /// (Violating the FluentValidation NotEmpty rule).
    /// </summary>
    public static CancelSaleCommand GenerateInvalidCommand_EmptySaleId()
    {
        return new CancelSaleCommand(Guid.Empty);
    }

    /// <summary>
    /// Provides the sale ID used for the valid command (for use in the Arrange phase).
    /// </summary>
    public static Guid ValidIdForArrangement() => ValidSaleId;
}
