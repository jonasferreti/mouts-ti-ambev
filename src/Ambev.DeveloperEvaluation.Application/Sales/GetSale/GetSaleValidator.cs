﻿using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleValidator : AbstractValidator<GetSaleQuery>
{
    public GetSaleValidator()
    {
        RuleFor(item => item.Id)
            .NotEmpty().WithMessage("Sale ID is required.");
    }
}
