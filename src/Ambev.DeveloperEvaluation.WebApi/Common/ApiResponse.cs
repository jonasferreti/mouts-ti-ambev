using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class ApiResponse : ApiResponse<ValidationErrorDetail>
{
}


public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public IEnumerable<T> Errors { get; set; } = [];
}
