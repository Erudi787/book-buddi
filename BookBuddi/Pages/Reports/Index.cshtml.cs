using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services;

namespace BookBuddi.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly ReportService _reportService;

        public IndexModel(ReportService reportService)
        {
            _reportService = reportService;
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

            TotalBooks = await _reportService.GetTotalBooksCountAsync();
            AvailableBooks = await _reportService.GetAvailableBooksCountAsync();
            BorrowedBooks = await _reportService.GetBorrowedBooksCountAsync();
            TotalMembers = await _reportService.GetTotalMembersCountAsync();
            ActiveMembers = await _reportService.GetActiveMembersCountAsync();
            ActiveTransactions = await _reportService.GetActiveTransactionsCountAsync();
            OverdueTransactions = await _reportService.GetOverdueTransactionsCountAsync();
            UnpaidFinesAmount = await _reportService.GetTotalUnpaidFinesAmountAsync();
            UnpaidFinesCount = await _reportService.GetUnpaidFinesCountAsync();
            PendingRequests = await _reportService.GetPendingRequestsCountAsync();
            TotalRequests = await _reportService.GetTotalRequestsCountAsync();

            return Page();
        }
    }
}
