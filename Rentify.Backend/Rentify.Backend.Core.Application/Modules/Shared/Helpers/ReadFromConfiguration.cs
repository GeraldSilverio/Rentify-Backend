namespace Rentify.Backend.Core.Application.Modules.Shared.Helpers
{
    public static class ReadFromConfiguration
    {
        public static string GetValueFromConfig(string keyConfig)
        {
            string? value = Environment.GetEnvironmentVariable(keyConfig);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            throw new InvalidOperationException($"No se encontró configuración para '{keyConfig}' en el entorno.");
        }
    }
}
