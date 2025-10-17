using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleItemCommand that defines validation rules for each item in a sale.
/// </summary>
public class UpdateSaleItemValidator : AbstractValidator<UpdateSaleItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleItemValidator with defined validation rules.
    /// - ProductId: Must be a valid Guid and cannot be empty (Guid.Empty)
    /// - ProductName: Required (Length validation removed)
    /// - Quantity: Must be between 1 and Quantity.maxValue
    /// - UnitPrice: Must be greater than or equal to zero
    /// </summary>
    public UpdateSaleItemValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("Product Name is required.");

        RuleFor(item => item.Quantity)
            .InclusiveBetween(1, Quantity.MaxValue)
            .WithMessage($"Quantity must be between 1 and {Quantity.MaxValue}");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("Unit Price must be greater than zero.");
    }
}