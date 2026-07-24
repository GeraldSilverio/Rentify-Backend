using System.Text.Json;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;

namespace Rentify.Backend.Logging.Tests;

public sealed class FileLoggingTests
{
    [Fact]
    public void FileSinkCreatesDailyCompactJsonLog()
    {
        string temporaryDirectory = Directory.CreateTempSubdirectory("rentify-logging-tests-").FullName;

        try
        {
            string path = Path.Combine(temporaryDirectory, "rentify-.json");

            using (Logger logger = new LoggerConfiguration()
                       .WriteTo.File(
                           new CompactJsonFormatter(),
                           path,
                           rollingInterval: RollingInterval.Day,
                           retainedFileCountLimit: 2,
                           retainedFileTimeLimit: TimeSpan.FromDays(1),
                           fileSizeLimitBytes: 1024 * 1024,
                           rollOnFileSizeLimit: true)
                       .CreateLogger())
            {
                logger.Information(
                    "File sink test with correlation {CorrelationId}",
                    "file-test-correlation");
            }

            string logFile = Assert.Single(Directory.GetFiles(
                temporaryDirectory,
                "rentify-*.json"));
            string line = Assert.Single(File.ReadAllLines(logFile));

            using JsonDocument document = JsonDocument.Parse(line);
            Assert.True(document.RootElement.TryGetProperty("@t", out _));
            Assert.True(document.RootElement.TryGetProperty("@mt", out _));
            Assert.Equal(
                "file-test-correlation",
                document.RootElement.GetProperty("CorrelationId").GetString());
        }
        finally
        {
            Directory.Delete(temporaryDirectory, recursive: true);
        }
    }
}
