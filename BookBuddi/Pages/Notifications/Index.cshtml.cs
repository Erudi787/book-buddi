using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;
using BookBuddi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Pages.Notifications
{
    public class IndexModel : PageModel
    {
        private readonly NotificationService _notificationService;
        private readonly ApplicationDbContext _context;

        public IndexModel(NotificationService notificationService, ApplicationDbContext context)
        {
            _notificationService = notificationService;
            _context = context;
        }

        public IEnumerable<Notification> Notifications { get; set; } = new List<Notification>();
        public List<Member> Members { get; set; } = new List<Member>();
        public int SelectedMemberId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? memberId)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            // Require login
            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToPage("/Account/Login");
            }

            if (userRole == "Member")
            {
                // Members only see their own notifications
                var sessionMemberId = HttpContext.Session.GetInt32("MemberId");
                if (sessionMemberId.HasValue)
                {
                    SelectedMemberId = sessionMemberId.Value;
                    Notifications = await _notificationService.GetMemberNotificationsAsync(sessionMemberId.Value);
                }
            }
            else
            {
                // Admin can select which member's notifications to view
                Members = await _context.Members.ToListAsync();

                if (memberId.HasValue && memberId.Value > 0)
                {
                    SelectedMemberId = memberId.Value;
                    Notifications = await _notificationService.GetMemberNotificationsAsync(memberId.Value);
                }
            }

            return Page();
        }
    }
}
