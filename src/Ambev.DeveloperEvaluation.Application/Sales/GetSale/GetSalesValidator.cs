using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSalesValidator : AbstractValidator<GetSalesQuery>
{
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;

    public GetSalesValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0).WithMessage("The page number (_page) must be a positive integer.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(MinPageSize, MaxPageSize)
            .WithMessage($"The page size (_size) must be between {MinPageSize} and {MaxPageSize}.");
    }
}