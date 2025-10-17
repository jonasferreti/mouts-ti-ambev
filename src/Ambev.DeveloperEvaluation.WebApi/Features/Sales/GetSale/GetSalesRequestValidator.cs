using Ambev.DeveloperEvaluation.Application.Helpers;
using Ambev.DeveloperEvaluation.Domain.Shared;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

public class GetSalesRequestValidator : AbstractValidator<GetSalesRequest>
{
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;

    public GetSalesRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0).WithMessage("The page number (_page) must be a positive integer.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(MinPageSize, MaxPageSize)
            .WithMessage($"The page size (_size) must be between {MinPageSize} and {MaxPageSize}.");

        RuleFor(request => request.SortField)
            .Must(ValidationHelpers.IsValidEnumName<SaleSortField>)
            .WithMessage(x => ValidationHelpers.GetInvalidEnumMessage<SaleSortField>(nameof(GetSalesRequest.SortField)))
            .When(x => !string.IsNullOrWhiteSpace(x.SortField));
            
        RuleFor(request => request.SortDirection)
            .Must(ValidationHelpers.IsValidSortDirection)
            .WithMessage(x => ValidationHelpers.GetInvalidSortDirectionMessage(nameof(GetSalesRequest.SortDirection)))
            .When(x => !string.IsNullOrWhiteSpace(x.SortDirection));

    }
}
