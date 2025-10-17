using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Validator for DeleteSaleItemRequest
/// </summary>
public class DeleteSaleItemRequestValidator : AbstractValidator<DeleteSaleItemRequest>
{
    /// <summary>
    /// Initializes validation rules for DeleteSaleItemRequest
    /// </summary>
    public DeleteSaleItemRequestValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("Sale ID is required");

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("Item ID is required");
    }
}

