namespace RentalAttireBackend.Application.Common
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public Result<T> Success(T data) => new() { Data = data, IsSuccess = true };
        public Result<T> Failure(T data) => new() { }
    }
}
