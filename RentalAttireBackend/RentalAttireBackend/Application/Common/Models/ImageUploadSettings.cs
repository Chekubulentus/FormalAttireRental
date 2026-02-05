using Microsoft.Extensions.ObjectPool;

namespace RentalAttireBackend.Application.Common.Models
{
    public class ImageUploadSettings
    {
        public string UploadDirectory { get; set; } = string.Empty;
        public long MaxFileSizeInBytes { get; set; }
        public string[] AllowedExtensions { get; set; } = null!;
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
    }
}
