using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public IEnumerable<BookViewModel> GetAllBooks()
        {
            var books = _bookRepository.GetBooks().ToList();
            return _mapper.Map<IEnumerable<BookViewModel>>(books);
        }

        public BookViewModel? GetBookById(int bookId)
        {
            var book = _bookRepository.GetBookById(bookId);
            return book != null ? _mapper.Map<BookViewModel>(book) : null;
        }

        public BookViewModel? GetBookByISBN(string isbn)
        {
            var book = _bookRepository.GetBookByISBN(isbn);
            return book != null ? _mapper.Map<BookViewModel>(book) : null;
        }

        public IEnumerable<BookViewModel> SearchBooks(string searchTerm)
        {
            var books = _bookRepository.SearchBooks(searchTerm);
            return _mapper.Map<IEnumerable<BookViewModel>>(books);
        }

        public IEnumerable<BookViewModel> GetBooksByCategory(int categoryId)
        {
            var books = _bookRepository.GetBooksByCategory(categoryId);
            return _mapper.Map<IEnumerable<BookViewModel>>(books);
        }

        public IEnumerable<BookViewModel> GetBooksByGenre(int genreId)
        {
            var books = _bookRepository.GetBooksByGenre(genreId);
            return _mapper.Map<IEnumerable<BookViewModel>>(books);
        }

        public IEnumerable<BookViewModel> GetBooksByAuthor(int authorId)
        {
            var books = _bookRepository.GetBooksByAuthor(authorId);
            return _mapper.Map<IEnumerable<BookViewModel>>(books);
        }

        public IEnumerable<BookViewModel> GetAvailableBooks()
        {
            var books = _bookRepository.GetAvailableBooks();
            return _mapper.Map<IEnumerable<BookViewModel>>(books);
        }

        public IEnumerable<BookViewModel> GetBooksByStatus(BookStatus status)
        {
            var books = _bookRepository.GetBooksByStatus(status);
            return _mapper.Map<IEnumerable<BookViewModel>>(books);
        }

        public bool BookExists(int bookId)
        {
            return _bookRepository.BookExists(bookId);
        }

        public bool ISBNExists(string isbn, int? excludeBookId = null)
        {
            var existingBook = _bookRepository.GetBookByISBN(isbn);
            if (existingBook == null) return false;
            if (excludeBookId.HasValue && existingBook.BookId == excludeBookId.Value) return false;
            return true;
        }

        public int GetTotalBooksCount()
        {
            return _bookRepository.GetTotalBooksCount();
        }

        public void AddBook(BookViewModel model, string createdBy)
        {
            // Validation
            if (ISBNExists(model.ISBN))
                throw new InvalidOperationException("A book with this ISBN already exists");

            var book = _mapper.Map<Book>(model);
            book.CreatedBy = createdBy;
            book.CreatedTime = DateTime.Now;
            book.UpdatedBy = createdBy;
            book.UpdatedTime = DateTime.Now;

            _bookRepository.AddBook(book);
        }

        public void UpdateBook(BookViewModel model, string updatedBy)
        {
            if (ISBNExists(model.ISBN, model.BookId))
                throw new InvalidOperationException("A book with this ISBN already exists");

            var book = _bookRepository.GetBookById(model.BookId);
            if (book == null)
                throw new InvalidOperationException("Book not found");

            // Map changes
            _mapper.Map(model, book);
            book.UpdatedBy = updatedBy;
            book.UpdatedTime = DateTime.Now;

            _bookRepository.UpdateBook(book);
        }

        public void DeleteBook(int bookId)
        {
            var book = _bookRepository.GetBookById(bookId);
            if (book == null)
                throw new InvalidOperationException("Book not found");

            _bookRepository.DeleteBook(book);
        }

        public void UpdateBookAvailability(int bookId, int change)
        {
            _bookRepository.UpdateBookAvailability(bookId, change);
        }
    }
}
