using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Common.Interfaces
{
    public interface IFileUploadService
    {
        public Task<FileUploadResult> UploadImageAsync(IFormFile file, string folder);
        public Task<bool> DeleteFileAsync(string filePath);
        public Task<byte[]> GetFileAsync(string filePath);
        public string GetFileUrl(string filePath);
    }
}
