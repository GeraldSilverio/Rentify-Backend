namespace Rentify.Backend.Shared.Storage;

public sealed class CloudinarySettings
{
    public string CloudName { get; }
    public string ApiKey { get; }
    public string ApiSecret { get; }

    private CloudinarySettings(string cloudName, string apiKey, string apiSecret)
    {
        CloudName = cloudName;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
    }

    public static CloudinarySettings FromEnvironment()
    {
        string cloudName = GetRequiredEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
        string apiKey = GetRequiredEnvironmentVariable("CLOUDINARY_API_KEY");
        string apiSecret = GetRequiredEnvironmentVariable("CLOUDINARY_API_SECRET");

        return new CloudinarySettings(cloudName, apiKey, apiSecret);
    }

    private static string GetRequiredEnvironmentVariable(string key)
    {
        string? value = Environment.GetEnvironmentVariable(key);

        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"{key} must be configured in the .env file.");

        return value;
    }
}
