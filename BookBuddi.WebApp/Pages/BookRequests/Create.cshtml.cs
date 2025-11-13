using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Data;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Pages.BookRequests
{
    public class CreateModel : PageModel
    {
        private readonly IBookRequestService _requestService;
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public CreateModel(IBookRequestService requestService, ApplicationDbContext context, INotificationService notificationService)
        {
            _requestService = requestService;
            _context = context;
            _notificationService = notificationService;
        }

        public List<Member> Members { get; set; } = new List<Member>();
        public string? ErrorMessage { get; set; }
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadNotificationCount { get; set; }

        public async Task OnGetAsync()
        {
            Members = await _context.Members.Where(m => m.Status == MemberStatus.Active).ToListAsync();
            
            // Load notifications for members
            var userRole = HttpContext.Session.GetString("UserRole");
            var memberId = HttpContext.Session.GetInt32("MemberId");
            if (userRole == "Member" && memberId.HasValue)
            {
                var allNotifications = _notificationService.GetNotificationsByMember(memberId.Value);
                RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);
            }
        }

        public async Task<IActionResult> OnPostAsync(int memberId, string bookTitle, string? author, string? isbn, string? notes)
        {
            try
            {
                var request = new BookRequestViewModel
                {
                    MemberId = memberId,
                    BookTitle = bookTitle,
                    AuthorName = author ?? string.Empty,
                    ISBN = isbn,
                    MemberNotes = notes,
                    RequestDate = DateTime.Now,
                    Status = RequestStatus.Pending
                };

                var createdBy = HttpContext.Session.GetString("MemberName") ?? "System";
                _requestService.AddRequest(request, createdBy);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Members = await _context.Members.Where(m => m.Status == MemberStatus.Active).ToListAsync();
                
                // Reload notifications
                var userRole = HttpContext.Session.GetString("UserRole");
                var memberIdSession = HttpContext.Session.GetInt32("MemberId");
                if (userRole == "Member" && memberIdSession.HasValue)
                {
                    var allNotifications = _notificationService.GetNotificationsByMember(memberIdSession.Value);
                    RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                    UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);
                }
                
                return Page();
            }
        }
    }
}
