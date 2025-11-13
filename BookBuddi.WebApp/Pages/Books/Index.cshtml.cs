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
        public string? Filter { get; set; }

        public void OnGet(string? searchTerm, string? filter)
        {
            SearchTerm = searchTerm;
            Filter = filter ?? "home";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Books = _bookService.SearchBooks(searchTerm);
            }
            else
            {
                var allBooks = _bookService.GetAllBooks().OrderByDescending(b => b.CreatedTime);
                
                Books = Filter?.ToLower() switch
                {
                    "new" => allBooks.Take(12).ToList(),
                    "popular" => allBooks.OrderByDescending(b => b.BookId).Take(12).ToList(),
                    _ => allBooks.Take(24).ToList() // home - show both sections
                };
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
