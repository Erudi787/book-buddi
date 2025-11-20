using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IBookRepository bookRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BookViewModel> GetAllBooks()
        {
            // Get all books (excluding archived)
            var books = _bookRepository.GetBooks()
                .Where(b => b.Status != BookStatus.Archived)
                .ToList();
            var bookViewModels = _mapper.Map<List<BookViewModel>>(books);

            // Get categories, genres, and authors to populate display names
            var categoryDict = _unitOfWork.Database.Set<Category>().ToDictionary(c => c.CategoryId, c => c.CategoryName);
            var genreDict = _unitOfWork.Database.Set<Genre>().ToDictionary(g => g.GenreId, g => g.GenreName);

            // Get book authors
            var bookIds = books.Select(b => b.BookId).ToList();
            var bookAuthors = _unitOfWork.Database.Set<BookAuthor>()
                .Where(ba => bookIds.Contains(ba.BookId))
                .ToList();

            var authorIds = bookAuthors.Select(ba => ba.AuthorId).Distinct().ToList();
            var authors = _unitOfWork.Database.Set<Author>()
                .Where(a => authorIds.Contains(a.AuthorId))
                .ToDictionary(a => a.AuthorId, a => $"{a.FirstName} {a.LastName}");

            // Group book authors by BookId
            var bookAuthorsGrouped = bookAuthors
                .GroupBy(ba => ba.BookId)
                .ToDictionary(g => g.Key, g => g.Select(ba => ba.AuthorId).ToList());

            // Populate display properties
            foreach (var bookViewModel in bookViewModels)
            {
                if (categoryDict.ContainsKey(bookViewModel.CategoryId))
                {
                    bookViewModel.CategoryName = categoryDict[bookViewModel.CategoryId];
                }

                if (genreDict.ContainsKey(bookViewModel.GenreId))
                {
                    bookViewModel.GenreName = genreDict[bookViewModel.GenreId];
                }

                if (bookAuthorsGrouped.ContainsKey(bookViewModel.BookId))
                {
                    var authorNames = bookAuthorsGrouped[bookViewModel.BookId]
                        .Where(authorId => authors.ContainsKey(authorId))
                        .Select(authorId => authors[authorId]);
                    bookViewModel.AuthorNames = string.Join(", ", authorNames);
                }
            }

            return bookViewModels;
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

            // Check if the book has any borrow transaction history
            var hasTransactions = _unitOfWork.Database.Set<BorrowTransaction>()
                .Any(bt => bt.BookId == bookId);

            if (hasTransactions)
            {
                // Soft delete: Archive the book instead of deleting it
                book.Status = BookStatus.Archived;
                book.UpdatedTime = DateTime.Now;
                _bookRepository.UpdateBook(book);
            }
            else
            {
                // Hard delete: No transaction history, safe to remove completely
                _bookRepository.DeleteBook(book);
            }
        }

        public void UpdateBookAvailability(int bookId, int change)
        {
            _bookRepository.UpdateBookAvailability(bookId, change);
        }
    }
}
