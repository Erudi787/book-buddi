using Microsoft.AspNetCore.Http;

namespace BookBuddi.Services.Interfaces
{
    public interface IFileUploadService
    {
        /// <summary>
        /// Uploads a book cover image and returns the relative path
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <returns>Relative path to the uploaded file (e.g., "/images/books/filename.jpg")</returns>
        Task<string> UploadBookCoverAsync(IFormFile file);

        /// <summary>
        /// Deletes a file from the file system
        /// </summary>
        /// <param name="filePath">Relative path to the file (e.g., "/images/books/filename.jpg")</param>
        Task DeleteFileAsync(string filePath);
    }
}
