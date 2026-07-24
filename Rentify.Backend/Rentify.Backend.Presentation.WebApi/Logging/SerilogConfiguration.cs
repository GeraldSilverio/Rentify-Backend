namespace Rentify.Backend.Presentation.WebApi.Logging;

public static class SerilogConfiguration
{
    public static void EnsureFileSinkDirectories(
        IConfiguration configuration,
        string contentRootPath)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentRootPath);

        IEnumerable<IConfigurationSection> fileSinks = configuration
            .GetSection("Serilog:WriteTo")
            .GetChildren()
            .Where(section => string.Equals(
                section["Name"],
                "File",
                StringComparison.OrdinalIgnoreCase));

        foreach (IConfigurationSection fileSink in fileSinks)
        {
            string? configuredPath = fileSink["Args:path"];
            if (string.IsNullOrWhiteSpace(configuredPath))
                throw new InvalidOperationException("The Serilog file sink path is required.");

            string absolutePath = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.GetFullPath(Path.Combine(contentRootPath, configuredPath));

            string? directory = Path.GetDirectoryName(absolutePath);
            if (string.IsNullOrWhiteSpace(directory))
                throw new InvalidOperationException("The Serilog file sink directory is invalid.");

            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (Exception exception) when (
                exception is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                throw new InvalidOperationException(
                    $"The configured Serilog directory '{directory}' could not be created.",
                    exception);
            }
        }
    }
}
