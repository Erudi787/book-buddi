using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Data;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IBookService _bookService;

    public IndexModel(ApplicationDbContext context, IBookService bookService)
    {
        _context = context;
        _bookService = bookService;
    }

    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveTransactions { get; set; }
    public int UnpaidFines { get; set; }
    public List<BookViewModel> SearchResults { get; set; } = new List<BookViewModel>();
    public List<BookViewModel> RecentBooks { get; set; } = new List<BookViewModel>();
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? GenreId { get; set; }
    public List<Data.Models.Category> Categories { get; set; } = new List<Data.Models.Category>();
    public List<Data.Models.Genre> Genres { get; set; } = new List<Data.Models.Genre>();

    public async Task<IActionResult> OnGetAsync(string? searchTerm, int? categoryId, int? genreId)
    {
        SearchTerm = searchTerm;
        CategoryId = categoryId;
        GenreId = genreId;

        // Load categories and genres for filter dropdowns
        Categories = await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
        Genres = await _context.Genres.OrderBy(g => g.GenreName).ToListAsync();

        // Check if any filters are applied
        bool hasFilters = !string.IsNullOrWhiteSpace(searchTerm) || categoryId.HasValue || genreId.HasValue;

        if (hasFilters)
        {
            // Use advanced search with filters
            var allBooks = _bookService.GetAllBooks();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                allBooks = allBooks.Where(b =>
                    b.BookTitle.ToLower().Contains(searchTerm) ||
                    (b.ISBN != null && b.ISBN.Contains(searchTerm)) ||
                    (b.Publisher != null && b.Publisher.ToLower().Contains(searchTerm)) ||
                    (b.Description != null && b.Description.ToLower().Contains(searchTerm)) ||
                    (b.AuthorNames != null && b.AuthorNames.ToLower().Contains(searchTerm)) ||
                    (b.CategoryName != null && b.CategoryName.ToLower().Contains(searchTerm)) ||
                    (b.GenreName != null && b.GenreName.ToLower().Contains(searchTerm))
                );
            }

            // Apply category filter
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                allBooks = allBooks.Where(b => b.CategoryId == categoryId.Value);
            }

            // Apply genre filter
            if (genreId.HasValue && genreId.Value > 0)
            {
                allBooks = allBooks.Where(b => b.GenreId == genreId.Value);
            }

            SearchResults = allBooks.ToList();
        }

        // Get recent books for the landing page
        RecentBooks = _bookService.GetAllBooks().Take(8).ToList();

        // Directly query the database for dashboard stats
        TotalBooks = await _context.Books.CountAsync();
        AvailableBooks = await _context.Books.CountAsync(b => b.Status == BookStatus.Available);
        TotalMembers = await _context.Members.CountAsync();
        ActiveTransactions = await _context.BorrowTransactions.CountAsync(t => t.Status == TransactionStatus.Active);
        UnpaidFines = await _context.Fines.CountAsync(f => f.Status == FineStatus.Unpaid);

        return Page();
    }

    // AJAX endpoint for smooth search
    public JsonResult OnGetSearchBooks(string? searchTerm, int? categoryId, int? genreId)
    {
        var allBooks = _bookService.GetAllBooks();

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            allBooks = allBooks.Where(b =>
                b.BookTitle.ToLower().Contains(searchTerm) ||
                (b.ISBN != null && b.ISBN.Contains(searchTerm)) ||
                (b.Publisher != null && b.Publisher.ToLower().Contains(searchTerm)) ||
                (b.Description != null && b.Description.ToLower().Contains(searchTerm)) ||
                (b.AuthorNames != null && b.AuthorNames.ToLower().Contains(searchTerm)) ||
                (b.CategoryName != null && b.CategoryName.ToLower().Contains(searchTerm)) ||
                (b.GenreName != null && b.GenreName.ToLower().Contains(searchTerm))
            );
        }

        // Apply category filter
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            allBooks = allBooks.Where(b => b.CategoryId == categoryId.Value);
        }

        // Apply genre filter
        if (genreId.HasValue && genreId.Value > 0)
        {
            allBooks = allBooks.Where(b => b.GenreId == genreId.Value);
        }

        var results = allBooks.Select(b => new
        {
            b.BookId,
            b.BookTitle,
            b.AuthorNames,
            b.CategoryName,
            b.GenreName,
            b.CoverImageUrl,
            b.AvailableCopies
        }).ToList();

        return new JsonResult(new { success = true, books = results, count = results.Count });
    }
}
