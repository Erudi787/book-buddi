using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Borrowing
{
    public class IndexModel : PageModel
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;

        public IndexModel(IBorrowingService borrowingService, IBookService bookService, IMemberService memberService)
        {
            _borrowingService = borrowingService;
            _bookService = bookService;
            _memberService = memberService;
        }

        public List<TransactionWithDetailsViewModel> Transactions { get; set; } = new List<TransactionWithDetailsViewModel>();

        public IActionResult OnGet()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            // Require login
            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToPage("/Account/Login");
            }

            IEnumerable<BorrowTransactionViewModel> transactionList;

            if (userRole == "Member")
            {
                // Show only the member's transactions
                var memberId = HttpContext.Session.GetInt32("MemberId");
                if (memberId.HasValue)
                {
                    transactionList = _borrowingService.GetTransactionsByMember(memberId.Value);
                }
                else
                {
                    transactionList = new List<BorrowTransactionViewModel>();
                }
            }
            else
            {
                // Admin sees all transactions
                transactionList = _borrowingService.GetAllTransactions();
            }

            // Enrich transactions with book and member details
            Transactions = transactionList.Select(t =>
            {
                var book = _bookService.GetBookById(t.BookId);
                var member = _memberService.GetMemberById(t.MemberId);
                return new TransactionWithDetailsViewModel
                {
                    Transaction = t,
                    BookTitle = book?.BookTitle ?? "Unknown",
                    MemberName = member != null ? $"{member.FirstName} {member.LastName}" : "Unknown"
                };
            }).ToList();

            return Page();
        }

        public class TransactionWithDetailsViewModel
        {
            public BorrowTransactionViewModel Transaction { get; set; } = new BorrowTransactionViewModel();
            public string BookTitle { get; set; } = string.Empty;
            public string MemberName { get; set; } = string.Empty;
        }
    }
}
