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
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return this.GetDbSet<Book>().OrderBy(b => b.BookTitle).ToList();
            }

            searchTerm = searchTerm.ToLower();

            var categories = this.GetDbSet<Category>();
            var genres = this.GetDbSet<Genre>();
            var bookAuthors = this.GetDbSet<BookAuthor>();
            var authors = this.GetDbSet<Author>();

            // Find books that match by title, ISBN, publisher, or description
            var booksByDirectMatch = this.GetDbSet<Book>()
                .Where(b =>
                    b.BookTitle.ToLower().Contains(searchTerm) ||
                    b.ISBN.Contains(searchTerm) ||
                    (b.Publisher != null && b.Publisher.ToLower().Contains(searchTerm)) ||
                    (b.Description != null && b.Description.ToLower().Contains(searchTerm))
                );

            // Find books by category name
            var booksByCategoryName = from b in this.GetDbSet<Book>()
                                      join c in categories on b.CategoryId equals c.CategoryId
                                      where c.CategoryName.ToLower().Contains(searchTerm)
                                      select b;

            // Find books by genre name
            var booksByGenreName = from b in this.GetDbSet<Book>()
                                   join g in genres on b.GenreId equals g.GenreId
                                   where g.GenreName.ToLower().Contains(searchTerm)
                                   select b;

            // Find books by author name
            var booksByAuthorName = from b in this.GetDbSet<Book>()
                                    join ba in bookAuthors on b.BookId equals ba.BookId
                                    join a in authors on ba.AuthorId equals a.AuthorId
                                    where a.FirstName.ToLower().Contains(searchTerm) ||
                                          a.LastName.ToLower().Contains(searchTerm)
                                    select b;

            // Combine all results and remove duplicates
            var allResults = booksByDirectMatch
                .Union(booksByCategoryName)
                .Union(booksByGenreName)
                .Union(booksByAuthorName)
                .OrderBy(b => b.BookTitle)
                .ToList();

            return allResults;
        }

        public IEnumerable<Book> AdvancedSearchBooks(string? searchTerm, int? categoryId, int? genreId)
        {
            var booksQuery = this.GetDbSet<Book>().AsQueryable();

            // Apply category filter if provided
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                booksQuery = booksQuery.Where(b => b.CategoryId == categoryId.Value);
            }

            // Apply genre filter if provided
            if (genreId.HasValue && genreId.Value > 0)
            {
                booksQuery = booksQuery.Where(b => b.GenreId == genreId.Value);
            }

            // If no text search, return the filtered results
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return booksQuery.OrderBy(b => b.BookTitle).ToList();
            }

            // Apply text search with joins for author names
            searchTerm = searchTerm.ToLower();

            var bookAuthors = this.GetDbSet<BookAuthor>();
            var authors = this.GetDbSet<Author>();

            // Books matching by title, ISBN, publisher, or description
            var booksByDirectMatch = booksQuery
                .Where(b =>
                    b.BookTitle.ToLower().Contains(searchTerm) ||
                    b.ISBN.Contains(searchTerm) ||
                    (b.Publisher != null && b.Publisher.ToLower().Contains(searchTerm)) ||
                    (b.Description != null && b.Description.ToLower().Contains(searchTerm))
                );

            // Books matching by author name (with category/genre filters already applied)
            var booksByAuthorName = from b in booksQuery
                                    join ba in bookAuthors on b.BookId equals ba.BookId
                                    join a in authors on ba.AuthorId equals a.AuthorId
                                    where a.FirstName.ToLower().Contains(searchTerm) ||
                                          a.LastName.ToLower().Contains(searchTerm)
                                    select b;

            // Combine and remove duplicates
            var results = booksByDirectMatch
                .Union(booksByAuthorName)
                .OrderBy(b => b.BookTitle)
                .ToList();

            return results;
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
