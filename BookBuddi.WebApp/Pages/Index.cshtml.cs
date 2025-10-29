using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Data;
using BookBuddi.Resources.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveTransactions { get; set; }
    public int UnpaidFines { get; set; }

    public async Task OnGetAsync()
    {
        // Directly query the database for dashboard stats
        TotalBooks = await _context.Books.CountAsync();
        AvailableBooks = await _context.Books.CountAsync(b => b.Status == BookStatus.Available);
        TotalMembers = await _context.Members.CountAsync();
        ActiveTransactions = await _context.BorrowTransactions.CountAsync(t => t.Status == TransactionStatus.Active);
        UnpaidFines = await _context.Fines.CountAsync(f => f.Status == FineStatus.Unpaid);
    }
}
