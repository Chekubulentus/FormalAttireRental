namespace RentalAttireBackend.Application.Common.Models
{
    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
