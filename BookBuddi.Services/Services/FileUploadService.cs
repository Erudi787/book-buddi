using BookBuddi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BookBuddi.Services.Services
{
    public class FileUploadService : IFileUploadService
    {
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const string BookCoversFolder = "images/books";
        private readonly string _webRootPath;

        public FileUploadService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public async Task<string> UploadBookCoverAsync(IFormFile file)
        {
            // Validate file
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file provided");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException($"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024}MB");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"File type not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");
            }

            // Create directory if it doesn't exist
            var uploadPath = Path.Combine(_webRootPath, BookCoversFolder);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path for database storage
            return $"/{BookCoversFolder}/{uniqueFileName}";
        }

        public Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Task.CompletedTask;
            }

            try
            {
                // Remove leading slash if present
                var relativePath = filePath.TrimStart('/');
                var fullPath = Path.Combine(_webRootPath, relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw - file deletion failure shouldn't break the application
                Console.WriteLine($"Error deleting file {filePath}: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
