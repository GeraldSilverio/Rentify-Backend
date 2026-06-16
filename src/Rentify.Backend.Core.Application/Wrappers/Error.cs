using Microsoft.AspNetCore.Http;
using System.Net;

namespace Rentify.Backend.Core.Application.Wrappers;

public sealed class Error
{
    public int Code { get; set; } = default!;
    public string Message { get; set; } = default!;

    public static Error NotFound(string message)
        => new()
        {
            Code = StatusCodes.Status404NotFound,
            Message = message
        };

    public static Error Validation(string message)
        => new()
        {
            Code = StatusCodes.Status400BadRequest,
            Message = message
        };

    public static Error Conflict(string message)
        => new()
        {
            Code = StatusCodes.Status409Conflict,
            Message = message
        };

    public static Error Unauthorized(string message)
        => new()
        {
            Code = StatusCodes.Status401Unauthorized,
            Message = message
        };
    public static Error ForBidden(string message)
        => new()
        {
            Code = StatusCodes.Status403Forbidden,
            Message = message
        };

    public static Error InternalServer(string message)
        => new()
        {
            Code = StatusCodes.Status500InternalServerError,
            Message = message
        };
}