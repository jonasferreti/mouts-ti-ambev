using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System;
using System.Net;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        
        public ExceptionMiddleware(RequestDelegate next, 
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (DomainException ex)
            {
                await HandleDomainExceptionAsync(context, ex);
            }
            catch (NotFoundException ex)
            {
                await HandleNotFoundExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, "InternalServerError", ex);
            }
        }

        private  Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var response = new ApiResponse
            {
                Success = false,
                Message = "Validation Failed",
                Errors = exception.Errors
                    .Select(error => (ValidationErrorDetail)error)
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }

        private  Task HandleDomainExceptionAsync(HttpContext context, Exception exception)
        {
            return HandleExceptionAsync(context, "BussinessLogicError",
                exception, HttpStatusCode.BadRequest);
        }

        private  Task HandleNotFoundExceptionAsync(HttpContext context, Exception exception)
        {
            return HandleExceptionAsync(context, "NotFoundError",
                exception, HttpStatusCode.NotFound);
        }

        private Task HandleExceptionAsync(HttpContext context, string message, 
            Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ApiResponse<string>
            {
                Success = false,
                Message = message,
                Errors = [ex.Message]
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _logger.LogError("Exception thrown : {Message} - {StackTrace}", ex.Message, ex.StackTrace);

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
