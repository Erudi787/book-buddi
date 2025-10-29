using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;

namespace BookBuddi.Pages.Borrowing
{
    public class IndexModel : PageModel
    {
        private readonly BorrowingService _borrowingService;

        public IndexModel(BorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }

        public IEnumerable<BorrowTransaction> Transactions { get; set; } = new List<BorrowTransaction>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            // Require login
            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToPage("/Account/Login");
            }

            if (userRole == "Member")
            {
                // Show only the member's transactions
                var memberId = HttpContext.Session.GetInt32("MemberId");
                if (memberId.HasValue)
                {
                    Transactions = await _borrowingService.GetMemberTransactionsAsync(memberId.Value);
                }
            }
            else
            {
                // Admin sees all active transactions
                Transactions = await _borrowingService.GetActiveTransactionsAsync();
            }

            return Page();
        }
    }
}
