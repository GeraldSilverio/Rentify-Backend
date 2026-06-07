using Microsoft.AspNetCore.Http;
using System.Net;

namespace Rentify.Backend.Core.Application.Shared.Response;

public sealed class Error
{
    public int Code { get; set; } = default!;
    public string Message { get; set; } = default!;

    public static Error SetError(string message,int code)
        => new()
        {
            Code = code,
            Message = message
        };

}