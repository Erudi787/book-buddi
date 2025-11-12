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

            // Load notifications for members
            var userRole = HttpContext.Session.GetString("UserRole");
            var memberId = HttpContext.Session.GetInt32("MemberId");
            if (userRole == "Member" && memberId.HasValue)
            {
                var allNotifications = _notificationService.GetNotificationsByMember(memberId.Value);
                RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "System";
                _borrowingService.ReturnBook(id, adminName);
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
