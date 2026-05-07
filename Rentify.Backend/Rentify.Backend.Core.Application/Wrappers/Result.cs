namespace Rentify.Backend.Core.Application.Wrappers
{
    /// <summary>
    /// Class for the standard response for APIs.
    /// </summary>
    /// <typeparam name="T">The type of data returned in the response.</typeparam>
    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public Error? Error { get; }

        private Result(bool isSuccess, T? value, Error? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value)
            => new(true, value, null);

        public static Result<T> Failure(Error error)
            => new(false, default, error);
    }

}
