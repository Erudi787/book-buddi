using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int bookId);
        Task<Book?> GetBookByISBNAsync(string isbn);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
        Task<IEnumerable<Book>> GetBooksByGenreAsync(int genreId);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId);
        Task<IEnumerable<Book>> GetAvailableBooksAsync();
        Task<IEnumerable<Book>> GetBooksByStatusAsync(BookStatus status);

        // crud
        Task<Book> AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int bookId);

        Task<bool> BookExistsAsync(int bookId);
        Task<bool> ISBNExistsAsync(string isbn);
        Task<int> GetTotalBooksCountAsync();
        Task UpdateBookAvailablityAsync(int bookId, int change);
    }
}