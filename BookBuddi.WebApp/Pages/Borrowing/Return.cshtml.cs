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

            // Authorization check for Members - they can only return their own books
            if (userRole == "Member")
            {
                var memberId = HttpContext.Session.GetInt32("MemberId");
                if (!memberId.HasValue || Transaction.MemberId != memberId.Value)
                {
                    TempData["ErrorMessage"] = "You can only return books that you have borrowed.";
                    return RedirectToPage("/Borrowing/Index");
                }
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

            // Authorization check for Members - they can only return their own books
            if (userRole == "Member")
            {
                var transaction = _borrowingService.GetTransactionById(id);
                var memberId = HttpContext.Session.GetInt32("MemberId");

                if (transaction == null)
                {
                    return NotFound();
                }

                if (!memberId.HasValue || transaction.MemberId != memberId.Value)
                {
                    TempData["ErrorMessage"] = "You can only return books that you have borrowed.";
                    return RedirectToPage("/Borrowing/Index");
                }
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
