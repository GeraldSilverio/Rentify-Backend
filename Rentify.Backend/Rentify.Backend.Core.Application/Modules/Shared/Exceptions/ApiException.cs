using System.Globalization;

namespace Rentify.Backend.Core.Application.Modules.Shared.Exceptions
{
    public class ApiException : Exception
    {
        public int ErrorCode { get; set; }
        public string? Key { get; set; }

        public ApiException() : base()
        {
        }

        public ApiException(string message) : base(message)
        {

        }
        public ApiException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public ApiException(
            string message,
            int errorCode,
            string key) : base(message)
        {
            ErrorCode = errorCode;
            Key = key;
        }

        public ApiException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
