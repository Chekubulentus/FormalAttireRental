using Microsoft.Extensions.Options;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime;
using System.Text.RegularExpressions;

namespace RentalAttireBackend.Infrastructure.Persistence.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly ImageUploadSettings _imageUploadSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileUploadService
            (
            IOptions<ImageUploadSettings> settings,
            IWebHostEnvironment environment,
            IHttpContextAccessor accessor
            )
        {
            _imageUploadSettings = settings.Value;
            _webHostEnvironment = environment;
            _httpContextAccessor = accessor;
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);

            if (!File.Exists(fullPath))
                return null;

            return await File.ReadAllBytesAsync(fullPath);
        }

        public string GetFileUrl(string filePath)
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request is null)
                return filePath;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{filePath}";
        }

        public async Task<FileUploadResult> UploadImageAsync(IFormFile file, string folder)
        {
            try
            {
                // STEP 5.1: Validate the file
                var validation = ValidateFile(file);
                if (!validation.Success)
                    return validation;

                if (string.IsNullOrEmpty(_webHostEnvironment.WebRootPath))
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        ErrorMessage = "Web root path is not configured. Please ensure wwwroot folder exists."
                    };
                }

                if (string.IsNullOrEmpty(folder))
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        ErrorMessage = "Folder path cannot be empty."
                    };
                }

                // STEP 5.2: Generate unique filename
                var fileName = GenerateUniqueFileName(file.FileName);

                // STEP 5.3: Create directory path
                // Example: wwwroot/uploads/persons/1/
                var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder);
                Directory.CreateDirectory(folderPath);  // Create if doesn't exist

                // STEP 5.4: Full file path
                // Example: wwwroot/uploads/persons/1/profile_20241231143000_abc123.jpg
                var filePath = Path.Combine(folderPath, fileName);

                // STEP 5.5: Relative path (to save in database)
                // Example: uploads/persons/1/profile_20241231143000_abc123.jpg
                var relativePath = Path.Combine("uploads", folder, fileName).Replace("\\", "/");

                // STEP 5.6: Save the file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // STEP 5.7: Resize image if too large
                await ResizeImageIfNeeded(filePath);

                // STEP 5.8: Return success result
                return new FileUploadResult
                {
                    Success = true,
                    FileName = fileName,
                    FilePath = relativePath,
                    FileSize = file.Length,
                    ContentType = file.ContentType
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Upload failed: {ex.Message}"
                };
            }
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var fileName = Path.GetFileNameWithoutExtension(originalFileName);

            fileName = Regex.Replace(fileName, @"[^a-zA-Z0-9_-]", "");

            return $"{fileName}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
        }
        private FileUploadResult ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = "No file uploaded"
                };
            }

            // Check file size
            if (file.Length > _imageUploadSettings.MaxFileSizeInBytes)
            {
                var maxSizeMB = _imageUploadSettings.MaxFileSizeInBytes / 1024 / 1024;
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = $"File size exceeds {maxSizeMB}MB limit"
                };
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_imageUploadSettings.AllowedExtensions.Contains(extension))
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = $"File type not allowed. Allowed: {string.Join(", ", _imageUploadSettings.AllowedExtensions)}"
                };
            }

            return new FileUploadResult { Success = true };
        }
        private async Task ResizeImageIfNeeded(string filePath)
        {
            try
            {
                using var image = await Image.LoadAsync(filePath);

                if (image.Width > _imageUploadSettings.MaxWidth || image.Height > _imageUploadSettings.MaxHeight)
                {
                    // Calculate new dimensions maintaining aspect ratio
                    var ratioX = (double)_imageUploadSettings.MaxWidth / image.Width;
                    var ratioY = (double)_imageUploadSettings.MaxHeight / image.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    var newWidth = (int)(image.Width * ratio);
                    var newHeight = (int)(image.Height * ratio);

                    image.Mutate(x => x.Resize(newWidth, newHeight));
                    await image.SaveAsync(filePath);
                }
            }
            catch
            {
                // If resize fails, keep original
            }
        }
    }
}
