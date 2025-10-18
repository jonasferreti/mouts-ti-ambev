using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleItemValidator : AbstractValidator<DeleteSaleItemCommand>
{
    /// <summary>
    /// Initializes validation rules for DeleteSaleItemCommand
    /// </summary>
    public DeleteSaleItemValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("Sale ID is required");

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("Item ID is required");
    }
}
