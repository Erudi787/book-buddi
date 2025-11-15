using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Borrowing
{
    public class BorrowModel : PageModel
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IMemberService _memberService;
        private readonly IBookService _bookService;
        private readonly INotificationService _notificationService;

        public BorrowModel(IBorrowingService borrowingService, IMemberService memberService, IBookService bookService, INotificationService notificationService)
        {
            _borrowingService = borrowingService;
            _memberService = memberService;
            _bookService = bookService;
            _notificationService = notificationService;
        }

        public List<MemberViewModel> Members { get; set; } = new List<MemberViewModel>();
        public List<BookViewModel> AvailableBooks { get; set; } = new List<BookViewModel>();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public int? SelectedBookId { get; set; }
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadNotificationCount { get; set; }

        public IActionResult OnGet(int? bookId)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            Members = _memberService.GetMembersByStatus(BookBuddi.Resources.Constants.MemberStatus.Active).ToList();
            AvailableBooks = _bookService.GetAvailableBooks().ToList();
            SelectedBookId = bookId;

            return Page();
        }

        public IActionResult OnPost(int memberId, int bookId)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "System";
                _borrowingService.BorrowBook(memberId, bookId, adminName);
                SuccessMessage = "Book borrowed successfully!";

                // Reload dropdowns
                Members = _memberService.GetMembersByStatus(BookBuddi.Resources.Constants.MemberStatus.Active).ToList();
                AvailableBooks = _bookService.GetAvailableBooks().ToList();

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                // Reload dropdowns
                Members = _memberService.GetMembersByStatus(BookBuddi.Resources.Constants.MemberStatus.Active).ToList();
                AvailableBooks = _bookService.GetAvailableBooks().ToList();

                return Page();
            }
        }
    }
}
