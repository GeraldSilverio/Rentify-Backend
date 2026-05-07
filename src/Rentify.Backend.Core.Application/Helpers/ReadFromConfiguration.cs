using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rentify.Backend.Core.Application.Helpers
{
    public static class ReadFromConfiguration
    {
        /// <summary>
        /// Metodo que se encarga de retonar el valor obtenido desde una var de enviroment o desde el config del aplicativo
        /// </summary>
        /// <param name="config"></param>
        /// <param name="keyConfig"></param>
        /// <returns></returns>
        public static string GetValueFromConfig(IConfiguration config, string keyConfig)
        {
            string? varConfig = config.GetValue<string>(keyConfig);
            if (!string.IsNullOrEmpty(varConfig))
            {
                return varConfig;
            }

            throw new InvalidOperationException($"No se encontró configuración para '{keyConfig}' en el entorno.");
        }
    }
}
