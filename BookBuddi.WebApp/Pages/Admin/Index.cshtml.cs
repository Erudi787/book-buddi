using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Data;
using BookBuddi.Resources.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Inventory Statistics
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }
        public int LostBooks { get; set; }
        public int DamagedBooks { get; set; }

        // Member Statistics
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int SuspendedMembers { get; set; }
        public int InactiveMembers { get; set; }

        // Transaction Statistics
        public int ActiveTransactions { get; set; }
        public int OverdueTransactions { get; set; }
        public int CompletedTransactionsToday { get; set; }
        public int TotalTransactions { get; set; }

        // Financial Statistics
        public decimal TotalUnpaidFines { get; set; }
        public int UnpaidFinesCount { get; set; }
        public decimal TotalPaidFinesToday { get; set; }
        public decimal TotalFinesCollected { get; set; }

        // Request Statistics
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalRequests { get; set; }

        // Recent Activity
        public int NewMembersThisWeek { get; set; }
        public int NewBooksThisWeek { get; set; }
        public int BorrowingsThisWeek { get; set; }

        // Categories and Genres
        public int TotalCategories { get; set; }
        public int TotalGenres { get; set; }
        public int TotalAuthors { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Account/Login");
            }

            var today = DateTime.Today;
            var weekAgo = today.AddDays(-7);

            // Inventory Statistics
            TotalBooks = await _context.Books.CountAsync();
            AvailableBooks = await _context.Books.CountAsync(b => b.Status == BookStatus.Available);
            BorrowedBooks = await _context.BorrowTransactions.CountAsync(t => t.Status == TransactionStatus.Active);
            LostBooks = 0; // Lost books tracking not implemented yet
            DamagedBooks = await _context.Books.CountAsync(b => b.Status == BookStatus.Unavailable) - BorrowedBooks;

            // Member Statistics
            TotalMembers = await _context.Members.CountAsync();
            ActiveMembers = await _context.Members.CountAsync(m => m.Status == MemberStatus.Active);
            SuspendedMembers = await _context.Members.CountAsync(m => m.Status == MemberStatus.Suspended);
            InactiveMembers = await _context.Members.CountAsync(m => m.Status == MemberStatus.Inactive);

            // Transaction Statistics
            ActiveTransactions = await _context.BorrowTransactions.CountAsync(t => t.Status == TransactionStatus.Active);
            OverdueTransactions = await _context.BorrowTransactions
                .CountAsync(t => t.Status == TransactionStatus.Active && t.DueDate < DateTime.Now);
            CompletedTransactionsToday = await _context.BorrowTransactions
                .CountAsync(t => t.Status == TransactionStatus.Returned && t.ReturnDate >= today);
            TotalTransactions = await _context.BorrowTransactions.CountAsync();

            // Financial Statistics
            TotalUnpaidFines = await _context.Fines
                .Where(f => f.Status == FineStatus.Unpaid)
                .SumAsync(f => (decimal?)f.Amount) ?? 0;
            UnpaidFinesCount = await _context.Fines.CountAsync(f => f.Status == FineStatus.Unpaid);
            TotalPaidFinesToday = await _context.Fines
                .Where(f => f.Status == FineStatus.Paid && f.UpdatedTime >= today)
                .SumAsync(f => (decimal?)f.Amount) ?? 0;
            TotalFinesCollected = await _context.Fines
                .Where(f => f.Status == FineStatus.Paid)
                .SumAsync(f => (decimal?)f.Amount) ?? 0;

            // Request Statistics
            PendingRequests = await _context.BookRequests.CountAsync(r => r.Status == RequestStatus.Pending);
            ApprovedRequests = await _context.BookRequests.CountAsync(r => r.Status == RequestStatus.Approved);
            RejectedRequests = await _context.BookRequests.CountAsync(r => r.Status == RequestStatus.Rejected);
            TotalRequests = await _context.BookRequests.CountAsync();

            // Recent Activity
            NewMembersThisWeek = await _context.Members.CountAsync(m => m.CreatedTime >= weekAgo);
            NewBooksThisWeek = await _context.Books.CountAsync(b => b.CreatedTime >= weekAgo);
            BorrowingsThisWeek = await _context.BorrowTransactions.CountAsync(t => t.BorrowDate >= weekAgo);

            // Categories and Genres
            TotalCategories = await _context.Set<Data.Models.Category>().CountAsync();
            TotalGenres = await _context.Set<Data.Models.Genre>().CountAsync();
            TotalAuthors = await _context.Set<Data.Models.Author>().CountAsync();

            return Page();
        }
    }
}
