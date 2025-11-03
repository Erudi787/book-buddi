using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly INotificationService _notificationService;

        public IndexModel(IBookService bookService, INotificationService notificationService)
        {
            _bookService = bookService;
            _notificationService = notificationService;
        }

        public IEnumerable<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadNotificationCount { get; set; }
        public string? SearchTerm { get; set; }

        public void OnGet(string? searchTerm)
        {
            SearchTerm = searchTerm;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Books = _bookService.SearchBooks(searchTerm);
            }
            else
            {
                Books = _bookService.GetAllBooks();
            }

            // Fetch notifications for logged-in member
            var memberId = HttpContext.Session.GetInt32("MemberId");
            if (memberId.HasValue)
            {
                var allNotifications = _notificationService.GetNotificationsByMember(memberId.Value);
                RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);
            }
        }
    }
}
