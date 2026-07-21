using Microsoft.AspNetCore.Http;
using System.Net;

namespace Rentify.Backend.Core.Application.Modules.Shared.Response;

public sealed class Error
{
    public int Code { get; set; } = default!;
    public string? Key { get; set; }
    public string Message { get; set; } = default!;

    public static Error SetError(
        string message,
        int code,
        string? key = null)
        => new()
        {
            Code = code,
            Key = key,
            Message = message
        };
}
