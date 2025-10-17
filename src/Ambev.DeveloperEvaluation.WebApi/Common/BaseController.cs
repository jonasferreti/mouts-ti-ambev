using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Common.Validation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using OneOf.Types;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NullReferenceException());

    protected string GetCurrentUserEmail() =>
        User.FindFirst(ClaimTypes.Email)?.Value ?? throw new NullReferenceException();

    protected IActionResult Ok<T>(T data) =>
            base.Ok(new ApiResponseWithData<T> { Data = data, Success = true });

    protected IActionResult Ok<T>(string message, T data) =>
            base.Ok(new ApiResponseWithData<T> { Data = data, Success = true, Message = message });

    protected IActionResult Ok(string message) =>
        base.Ok(new ApiResponse { Message = message, Success = true });

    protected IActionResult Created<T>(string routeName, object routeValues, T data) =>
        base.CreatedAtRoute(routeName, routeValues, new ApiResponseWithData<T> { Data = data, Success = true });

    protected IActionResult BadRequest(string message) =>
        base.BadRequest(new ApiResponse { Message = message, Success = false });

    protected IActionResult BadRequest(List<ValidationFailure> errors) =>
        base.BadRequest(new ApiResponse 
        { 
            Message = "Validation failed",
            Errors = errors.Select(o => (ValidationErrorDetail)o),
            Success = false
        });

    protected IActionResult NotFound(string message = "Resource not found") =>
        base.NotFound(new ApiResponse { Message = message, Success = false });

    protected IActionResult OkPaginated<T>(PaginatedList<T> pagedList) =>
        base.Ok(new PaginatedResponse<T>
        {
            Data = pagedList.Data,
            CurrentPage = pagedList.CurrentPage,
            TotalPages = pagedList.TotalPages,
            TotalCount = pagedList.TotalCount,
            Success = true
        });
}
