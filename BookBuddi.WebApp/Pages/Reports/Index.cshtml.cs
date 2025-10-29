using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Data;
using BookBuddi.Resources.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int ActiveTransactions { get; set; }
        public int OverdueTransactions { get; set; }
        public decimal UnpaidFinesAmount { get; set; }
        public int UnpaidFinesCount { get; set; }
        public int PendingRequests { get; set; }
        public int TotalRequests { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Index");
            }

            // Directly query the database for reports (ReportService not yet implemented)
            TotalBooks = await _context.Books.CountAsync();
            AvailableBooks = await _context.Books.CountAsync(b => b.Status == BookStatus.Available);
            BorrowedBooks = await _context.BorrowTransactions.CountAsync(t => t.Status == TransactionStatus.Active);
            TotalMembers = await _context.Members.CountAsync();
            ActiveMembers = await _context.Members.CountAsync(m => m.Status == MemberStatus.Active);
            ActiveTransactions = await _context.BorrowTransactions.CountAsync(t => t.Status == TransactionStatus.Active);
            OverdueTransactions = await _context.BorrowTransactions.CountAsync(t => t.Status == TransactionStatus.Active && t.DueDate < DateTime.Now);
            UnpaidFinesAmount = await _context.Fines.Where(f => f.Status == FineStatus.Unpaid).SumAsync(f => (decimal?)f.Amount) ?? 0;
            UnpaidFinesCount = await _context.Fines.CountAsync(f => f.Status == FineStatus.Unpaid);
            PendingRequests = await _context.BookRequests.CountAsync(r => r.Status == RequestStatus.Pending);
            TotalRequests = await _context.BookRequests.CountAsync();

            return Page();
        }
    }
}
