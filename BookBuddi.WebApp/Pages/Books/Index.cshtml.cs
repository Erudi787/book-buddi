using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _context;

        public IndexModel(IBookService bookService, INotificationService notificationService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _notificationService = notificationService;
            _context = context;
        }

        public IEnumerable<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadNotificationCount { get; set; }
        public string? SearchTerm { get; set; }
        public string? Filter { get; set; }
        public int? CategoryId { get; set; }
        public int? GenreId { get; set; }
        public List<Data.Models.Category> Categories { get; set; } = new List<Data.Models.Category>();
        public List<Data.Models.Genre> Genres { get; set; } = new List<Data.Models.Genre>();

        public async Task OnGetAsync(string? searchTerm, string? filter, int? categoryId, int? genreId)
        {
            SearchTerm = searchTerm;
            Filter = filter ?? "home";
            CategoryId = categoryId;
            GenreId = genreId;

            // Load categories and genres for filter dropdowns
            Categories = await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            Genres = await _context.Genres.OrderBy(g => g.GenreName).ToListAsync();

            // Check if any filters are applied
            bool hasFilters = !string.IsNullOrWhiteSpace(searchTerm) || categoryId.HasValue || genreId.HasValue;

            if (hasFilters)
            {
                // Use advanced search with filters
                var allBooks = _bookService.GetAllBooks();

                // Apply search term filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    allBooks = allBooks.Where(b =>
                        b.BookTitle.ToLower().Contains(searchTerm) ||
                        (b.ISBN != null && b.ISBN.Contains(searchTerm)) ||
                        (b.Publisher != null && b.Publisher.ToLower().Contains(searchTerm)) ||
                        (b.Description != null && b.Description.ToLower().Contains(searchTerm)) ||
                        (b.AuthorNames != null && b.AuthorNames.ToLower().Contains(searchTerm)) ||
                        (b.CategoryName != null && b.CategoryName.ToLower().Contains(searchTerm)) ||
                        (b.GenreName != null && b.GenreName.ToLower().Contains(searchTerm))
                    );
                }

                // Apply category filter
                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    allBooks = allBooks.Where(b => b.CategoryId == categoryId.Value);
                }

                // Apply genre filter
                if (genreId.HasValue && genreId.Value > 0)
                {
                    allBooks = allBooks.Where(b => b.GenreId == genreId.Value);
                }

                Books = allBooks.ToList();
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
