using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleRequestValidator with defined validation rules.
    /// - CustomerId: Must be a valid Guid and cannot be empty
    /// - CustomerName: Required (Length validation removed)
    /// - BranchId: Must be a valid Guid and cannot be empty
    /// - BranchName: Required (Length validation removed)
    /// - Items: Cannot be empty and must have at least one valid item
    /// </summary>
    public UpdateSaleRequestValidator()
    {
        RuleFor(sale => sale.Id)
           .NotEmpty().WithMessage("Sale ID is required.");

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

        RuleForEach(sale => sale.Items).SetValidator(new UpdateSaleItemRequestValidator());
    }
}


