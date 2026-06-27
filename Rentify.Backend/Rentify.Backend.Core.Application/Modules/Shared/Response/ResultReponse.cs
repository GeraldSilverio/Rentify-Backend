namespace Rentify.Backend.Core.Application.Modules.Shared.Response
{
    /// <summary>
    /// Class for the standard response for APIs.
    /// </summary>
    /// <typeparam name="T">The type of data returned in the response.</typeparam>
    public sealed class ResultReponse<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public Error? Error { get; }

        private ResultReponse(bool isSuccess, T? value, Error? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static ResultReponse<T> Success(T value)
            => new(true, value, null);

        public static ResultReponse<T> Failure(Error error)
            => new(false, default, error);
    }

}
