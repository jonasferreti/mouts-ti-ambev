using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand that defines validation rules for the sale creation command.
/// </summary>
public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleCommandValidator with defined validation rules.
    /// - CustomerId: Must be a valid Guid and cannot be empty
    /// - CustomerName: Required (Length validation removed)
    /// - BranchId: Must be a valid Guid and cannot be empty
    /// - BranchName: Required (Length validation removed)
    /// - Items: Cannot be empty and must have at least one valid item
    /// </summary>
    public CreateSaleValidator()
    {
        RuleFor(sale => sale.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(sale => sale.CustomerName)
            .NotEmpty().WithMessage("Customer Name is required.");

        RuleFor(sale => sale.BranchId)
            .NotEmpty().WithMessage("Branch ID is required.");

        RuleFor(sale => sale.BranchName)
            .NotEmpty().WithMessage("Branch Name is required.");

        RuleFor(sale => sale.Items)
            .NotEmpty().WithMessage("Sale must contain at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new CreateSaleItemValidator());
    }
}
