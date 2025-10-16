using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;


/// <summary>
/// Controller for managing sales operations
/// </summary>
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    /// <param name="request">The sale creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        //TODO: set created url
        return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = _mapper.Map<CreateSaleResponse>(response)
        });
    }
}