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

        public void OnGet(int? bookId)
        {
            Members = _memberService.GetMembersByStatus(BookBuddi.Resources.Constants.MemberStatus.Active).ToList();
            AvailableBooks = _bookService.GetAvailableBooks().ToList();
            SelectedBookId = bookId;

            // Load notifications for members
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Member")
            {
                var memberId = HttpContext.Session.GetInt32("MemberId");
                if (memberId.HasValue)
                {
                    var allNotifications = _notificationService.GetNotificationsByMember(memberId.Value);
                    RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                    UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);
                }
            }
        }

        public IActionResult OnPost(int memberId, int bookId)
        {
            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "System";
                _borrowingService.BorrowBook(memberId, bookId, adminName);
                SuccessMessage = "Book borrowed successfully!";

                // Reload dropdowns
                Members = _memberService.GetMembersByStatus(BookBuddi.Resources.Constants.MemberStatus.Active).ToList();
                AvailableBooks = _bookService.GetAvailableBooks().ToList();

                // Reload notifications
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "Member")
                {
                    var memberIdSession = HttpContext.Session.GetInt32("MemberId");
                    if (memberIdSession.HasValue)
                    {
                        var allNotifications = _notificationService.GetNotificationsByMember(memberIdSession.Value);
                        RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                        UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                // Reload dropdowns
                Members = _memberService.GetMembersByStatus(BookBuddi.Resources.Constants.MemberStatus.Active).ToList();
                AvailableBooks = _bookService.GetAvailableBooks().ToList();

                // Reload notifications
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "Member")
                {
                    var memberIdSession = HttpContext.Session.GetInt32("MemberId");
                    if (memberIdSession.HasValue)
                    {
                        var allNotifications = _notificationService.GetNotificationsByMember(memberIdSession.Value);
                        RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                        UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);
                    }
                }

                return Page();
            }
        }
    }
}
