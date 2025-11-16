using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Borrowing
{
    public class ReturnModel : PageModel
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;
        private readonly INotificationService _notificationService;

        public ReturnModel(IBorrowingService borrowingService, IBookService bookService, IMemberService memberService, INotificationService notificationService)
        {
            _borrowingService = borrowingService;
            _bookService = bookService;
            _memberService = memberService;
            _notificationService = notificationService;
        }

        public BorrowTransactionViewModel? Transaction { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadNotificationCount { get; set; }

        public IActionResult OnGet(int id)
        {
            // Authentication check - both Admin and Member can access
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToPage("/Account/Login");
            }

            Transaction = _borrowingService.GetTransactionById(id);

            if (Transaction == null)
            {
                return NotFound();
            }

            // Get book title
            var book = _bookService.GetBookById(Transaction.BookId);
            if (book != null)
            {
                BookTitle = book.BookTitle;
            }

            // Get member name
            var member = _memberService.GetMemberById(Transaction.MemberId);
            if (member != null)
            {
                MemberName = $"{member.FirstName} {member.LastName}";
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            // Authentication check - both Admin and Member can access
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var processedBy = userRole == "Admin"
                    ? HttpContext.Session.GetString("AdminName") ?? "Admin"
                    : HttpContext.Session.GetString("MemberName") ?? "Member";
                _borrowingService.ReturnBook(id, processedBy);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Transaction = _borrowingService.GetTransactionById(id);
                return Page();
            }
        }
    }
}
