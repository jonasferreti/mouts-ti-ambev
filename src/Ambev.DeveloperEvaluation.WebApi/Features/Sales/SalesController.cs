using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
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
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Cancels a specific Sale and all of its associated items, enforcing cascading domain rules.
    /// </summary>
    /// <param name="id">The GUID of the Sale to be cancelled.</param>
    /// <returns>Success response if the sale was cancelled</returns>
<<<<<<< Updated upstream
    [HttpPost("{id:guid}/cancel")] // <-- Alterado para PATCH
=======
    [HttpPatch("{Id}/cancel")]
>>>>>>> Stashed changes
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale(Guid id)
    {
        var command = new CancelSaleCommand(id);
        await _mediator.Send(command);

        return Ok("Sale cancelled successfully");
    }

    /// <summary>
    /// Cancels a specific Sale Item within a Sale. Triggers Sale cancellation if all items are cancelled.
    /// </summary>
    /// <param name="saleId">The GUID of the parent Sale.</param>
    /// <param name="itemId">The GUID of the item to be cancelled.</param>
    /// <returns>Success response if the sale item was cancelled</returns>
<<<<<<< Updated upstream
    [HttpPost("{saleId:guid}/items/{itemId:guid}/cancellation")]
=======
    [HttpPatch("{SaleId}/items/{ItemId}/cancel")]
>>>>>>> Stashed changes
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelSaleItem(Guid saleId, Guid itemId)
    {
        var command = new CancelSaleItemCommand(saleId, itemId);
        await _mediator.Send(command);

        return Ok("Sale item cancelled successfully");
    }
}