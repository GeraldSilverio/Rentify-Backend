namespace Rentify.Backend.Core.Application.Modules.Shared.Helpers;

public static class SlugGenerator
{
    public static string Generate(string value)
    {
        return value
            .Trim()
            .ToLower()
            .Replace(" ", "-");
    }
}