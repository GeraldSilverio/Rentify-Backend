using System.Text.RegularExpressions;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Validation;

public static class TenantRncRules
{
    private static readonly Regex DigitsOnlyRegex = new("^[0-9]+$", RegexOptions.Compiled);

    public static string? NormalizeOrNull(string? rnc)
    {
        if (string.IsNullOrWhiteSpace(rnc))
            return null;

        return rnc.Trim()
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty);
    }

    public static bool IsValidDominicanRnc(string? rnc)
    {
        string? normalizedRnc = NormalizeOrNull(rnc);

        return normalizedRnc is null ||
               (normalizedRnc.Length is 9 or 11 && DigitsOnlyRegex.IsMatch(normalizedRnc));
    }
}
