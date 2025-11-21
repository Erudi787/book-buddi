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
        private readonly IRatingService _ratingService;

        public DetailsModel(IBookService bookService, INotificationService notificationService, IRatingService ratingService)
        {
            _bookService = bookService;
            _notificationService = notificationService;
            _ratingService = ratingService;
        }

        public BookViewModel? Book { get; set; }
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadNotificationCount { get; set; }
        public BookRatingStatsViewModel? RatingStats { get; set; }
        public RatingViewModel? UserRating { get; set; }
        public bool CanRate { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Book = _bookService.GetBookById(id);

            if (Book == null)
            {
                return NotFound();
            }

            // Load rating stats
            RatingStats = await _ratingService.GetBookRatingStatsAsync(id);

            // Load notifications and ratings for members
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Member")
            {
                var memberId = HttpContext.Session.GetInt32("MemberId");
                if (memberId.HasValue)
                {
                    var allNotifications = _notificationService.GetNotificationsByMember(memberId.Value);
                    RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                    UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);

                    // Load user's rating if exists
                    UserRating = await _ratingService.GetMemberRatingForBookAsync(memberId.Value, id);
                    
                    // User can rate if logged in as member
                    CanRate = true;
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
