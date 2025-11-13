using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly INotificationService _notificationService;

        public DetailsModel(IBookService bookService, INotificationService notificationService)
        {
            _bookService = bookService;
            _notificationService = notificationService;
        }

        public BookViewModel? Book { get; set; }
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadNotificationCount { get; set; }

        public IActionResult OnGet(int id)
        {
            Book = _bookService.GetBookById(id);

            if (Book == null)
            {
                return NotFound();
            }

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

            return Page();
        }

        public IActionResult OnGetBookDetails(int id)
        {
            var book = _bookService.GetBookById(id);

            if (book == null)
            {
                return NotFound();
            }

            return new JsonResult(book);
        }
    }
}
