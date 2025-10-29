using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;

        public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllBooksAsync();
        }

        public async Task<Book?> GetBookDetailsAsync(int bookId)
        {
            return await _bookRepository.GetBookByIdAsync(bookId);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _bookRepository.GetAllBooksAsync();
            }

            return await _bookRepository.SearchBooksAsync(searchTerm);
        }

        public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
        {
            return await _bookRepository.GetAvailableBooksAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _bookRepository.GetBooksByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Book>> GetBooksByGenreAsync(int genreId)
        {
            return await _bookRepository.GetBooksByGenreAsync(genreId);
        }

        public async Task<bool> IsBookAvailableAsync(int bookId)
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            return book != null && book.Status == BookStatus.Available && book.AvailableCopies > 0;
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            // Validation
            if (await _bookRepository.ISBNExistsAsync(book.ISBN))
            {
                throw new InvalidOperationException($"A book with ISBN {book.ISBN} already exists.");
            }

            // Set initial values
            book.Status = book.AvailableCopies > 0 ? BookStatus.Available : BookStatus.Unavailable;
            book.DateAdded = DateTime.Now;

            return await _bookRepository.AddBookAsync(book);
        }

        public async Task UpdateBookAsync(Book book)
        {
            var existingBook = await _bookRepository.GetBookByIdAsync(book.BookId);
            if (existingBook == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            // Update status based on availability
            book.Status = book.AvailableCopies > 0 ? BookStatus.Available : BookStatus.Unavailable;
            book.LastModified = DateTime.Now;

            await _bookRepository.UpdateBookAsync(book);
        }

        public async Task DeleteBookAsync(int bookId)
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            // Check if book has active borrows (if AvailableCopies is 0, it means books are borrowed)
            if (book.AvailableCopies == 0)
            {
                throw new InvalidOperationException("Cannot delete a book that has active borrows.");
            }

            await _bookRepository.DeleteBookAsync(bookId);
        }

        public async Task<int> GetTotalBooksCountAsync()
        {
            return await _bookRepository.GetTotalBooksCountAsync();
        }
    }
}
