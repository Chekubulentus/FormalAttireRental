namespace RentalAttireBackend.Application.Common
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public static Result<T> Success(T data) => new() { Data = data, IsSuccess = true };
        public static Result<T> Failure(string message) => new() { IsSuccess = false, ErrorMessage = message };
        public static Result<T> SuccessWithMessage(string message) => new() { IsSuccess = true, SuccessMessage = message };
    }
}
