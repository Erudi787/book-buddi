using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .OrderBy(b => b.BookTitle)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int bookId)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.BookId == bookId);
        }

        public async Task<Book?> GetBookByISBNAsync(string isbn)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Where(b => b.BookTitle.Contains(searchTerm) ||
                           b.ISBN.Contains(searchTerm) ||
                           b.BookAuthors.Any(ba =>
                               ba.Author.FirstName.Contains(searchTerm) ||
                               ba.Author.LastName.Contains(searchTerm)))
                .OrderBy(b => b.BookTitle)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Where(b => b.CategoryId == categoryId)
                .OrderBy(b => b.BookTitle)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByGenreAsync(int genreId)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Where(b => b.GenreId == genreId)
                .OrderBy(b => b.BookTitle)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId))
                .OrderBy(b => b.BookTitle)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Where(b => b.Status == BookStatus.Available && b.AvailableCopies > 0)
                .OrderBy(b => b.BookTitle)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByStatusAsync(BookStatus status)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Where(b => b.Status == status)
                .OrderBy(b => b.BookTitle)
                .ToListAsync();
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            book.DateAdded = DateTime.Now;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task UpdateBookAsync(Book book)
        {
            book.LastModified = DateTime.Now;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> BookExistsAsync(int bookId)
        {
            return await _context.Books.AnyAsync(b => b.BookId == bookId);
        }

        public async Task<bool> ISBNExistsAsync(string isbn)
        {
            return await _context.Books.AnyAsync(b => b.ISBN == isbn);
        }

        public async Task<int> GetTotalBooksCountAsync()
        {
            return await _context.Books.CountAsync();
        }

        public async Task UpdateBookAvailabilityAsync(int bookId, int change)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book != null)
            {
                book.AvailableCopies += change;
                book.LastModified = DateTime.Now;

                // Update status based on availability
                book.Status = book.AvailableCopies > 0 ? BookStatus.Available : BookStatus.Unavailable;

                await _context.SaveChangesAsync();
            }
        }

        public Task UpdateBookAvailablityAsync(int bookId, int change)
        {
            throw new NotImplementedException();
        }
    }
}