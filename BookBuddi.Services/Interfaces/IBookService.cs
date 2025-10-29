using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.Interfaces
{
    public interface IBookService
    {
        IEnumerable<BookViewModel> GetAllBooks();
        BookViewModel? GetBookById(int bookId);
        BookViewModel? GetBookByISBN(string isbn);
        IEnumerable<BookViewModel> SearchBooks(string searchTerm);
        IEnumerable<BookViewModel> GetBooksByCategory(int categoryId);
        IEnumerable<BookViewModel> GetBooksByGenre(int genreId);
        IEnumerable<BookViewModel> GetBooksByAuthor(int authorId);
        IEnumerable<BookViewModel> GetAvailableBooks();
        IEnumerable<BookViewModel> GetBooksByStatus(BookStatus status);
        bool BookExists(int bookId);
        bool ISBNExists(string isbn, int? excludeBookId = null);
        int GetTotalBooksCount();
        void AddBook(BookViewModel model, string createdBy);
        void UpdateBook(BookViewModel model, string updatedBy);
        void DeleteBook(int bookId);
        void UpdateBookAvailability(int bookId, int change);
    }
}
