using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Interfaces
{
    public interface IBookRepository
    {
        IQueryable<Book> GetBooks();
        Book? GetBookById(int bookId);
        Book? GetBookByISBN(string isbn);
        IEnumerable<Book> SearchBooks(string searchTerm);
        IEnumerable<Book> GetBooksByCategory(int categoryId);
        IEnumerable<Book> GetBooksByGenre(int genreId);
        IEnumerable<Book> GetBooksByAuthor(int authorId);
        IEnumerable<Book> GetAvailableBooks();
        IEnumerable<Book> GetBooksByStatus(BookStatus status);
        bool BookExists(int bookId);
        bool ISBNExists(string isbn);
        int GetTotalBooksCount();
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(Book book);
        void UpdateBookAvailability(int bookId, int change);
    }
}
