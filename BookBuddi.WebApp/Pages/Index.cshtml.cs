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

    public async Task<IActionResult> OnGetAsync(string? searchTerm)
    {
        // Check if user is logged in as a Member, redirect to Books page
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole == "Member")
        {
            return RedirectToPage("/Books/Index");
        }

        SearchTerm = searchTerm;

        // If search term is provided, get search results
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var allBooks = _bookService.GetAllBooks();
            SearchResults = allBooks.Where(b => 
                b.BookTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (b.AuthorNames != null && b.AuthorNames.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (b.CategoryName != null && b.CategoryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (b.GenreName != null && b.GenreName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (b.ISBN != null && b.ISBN.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            ).ToList();
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
}
