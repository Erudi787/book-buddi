using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class BookRepository : BaseRepository, IBookRepository
    {
        public BookRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Book> GetBooks()
        {
            return this.GetDbSet<Book>().OrderBy(b => b.BookTitle);
        }

        public Book? GetBookById(int bookId)
        {
            return this.GetDbSet<Book>().FirstOrDefault(b => b.BookId == bookId);
        }

        public Book? GetBookByISBN(string isbn)
        {
            return this.GetDbSet<Book>().FirstOrDefault(b => b.ISBN == isbn);
        }

        public IEnumerable<Book> SearchBooks(string searchTerm)
        {
            return this.GetDbSet<Book>()
                .Where(b => b.BookTitle.Contains(searchTerm) ||
                           b.ISBN.Contains(searchTerm) ||
                           (b.Publisher != null && b.Publisher.Contains(searchTerm)))
                .OrderBy(b => b.BookTitle)
                .ToList();
        }

        public IEnumerable<Book> GetBooksByCategory(int categoryId)
        {
            return this.GetDbSet<Book>()
                .Where(b => b.CategoryId == categoryId)
                .OrderBy(b => b.BookTitle)
                .ToList();
        }

        public IEnumerable<Book> GetBooksByGenre(int genreId)
        {
            return this.GetDbSet<Book>()
                .Where(b => b.GenreId == genreId)
                .OrderBy(b => b.BookTitle)
                .ToList();
        }

        public IEnumerable<Book> GetBooksByAuthor(int authorId)
        {
            return this.GetDbSet<BookAuthor>()
                .Where(ba => ba.AuthorId == authorId)
                .Join(this.GetDbSet<Book>(),
                    ba => ba.BookId,
                    b => b.BookId,
                    (ba, b) => b)
                .OrderBy(b => b.BookTitle)
                .ToList();
        }

        public IEnumerable<Book> GetAvailableBooks()
        {
            return this.GetDbSet<Book>()
                .Where(b => b.Status == BookStatus.Available && b.AvailableCopies > 0)
                .OrderBy(b => b.BookTitle)
                .ToList();
        }

        public IEnumerable<Book> GetBooksByStatus(BookStatus status)
        {
            return this.GetDbSet<Book>()
                .Where(b => b.Status == status)
                .OrderBy(b => b.BookTitle)
                .ToList();
        }

        public bool BookExists(int bookId)
        {
            return this.GetDbSet<Book>().Any(b => b.BookId == bookId);
        }

        public bool ISBNExists(string isbn)
        {
            return this.GetDbSet<Book>().Any(b => b.ISBN == isbn);
        }

        public int GetTotalBooksCount()
        {
            return this.GetDbSet<Book>().Count();
        }

        public void AddBook(Book book)
        {
            this.GetDbSet<Book>().Add(book);
            UnitOfWork.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            SetEntityState(book, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteBook(Book book)
        {
            this.GetDbSet<Book>().Remove(book);
            UnitOfWork.SaveChanges();
        }

        public void UpdateBookAvailability(int bookId, int change)
        {
            var book = GetBookById(bookId);
            if (book != null)
            {
                book.AvailableCopies += change;
                book.Status = book.AvailableCopies > 0 ? BookStatus.Available : BookStatus.Unavailable;
                UpdateBook(book);
            }
        }
    }
}
