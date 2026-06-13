namespace Rentify.Backend.Presentation.WebApi.Configuration;

public static class EnvFileLoader
{
    public static void LoadFromNearest(string startDirectory)
    {
        DirectoryInfo? directory = new(startDirectory);

        while (directory is not null)
        {
            string envFilePath = Path.Combine(directory.FullName, ".env");

            if (File.Exists(envFilePath))
            {
                Load(envFilePath);
                return;
            }

            directory = directory.Parent;
        }
    }

    private static void Load(string envFilePath)
    {
        foreach (string line in File.ReadAllLines(envFilePath))
        {
            string trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
                continue;

            int separatorIndex = trimmedLine.IndexOf('=');

            if (separatorIndex <= 0)
                continue;

            string key = trimmedLine[..separatorIndex].Trim();
            string value = trimmedLine[(separatorIndex + 1)..].Trim().Trim('"');

            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
