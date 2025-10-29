using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Data;
using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Pages.Notifications
{
    public class IndexModel : PageModel
    {
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _context;

        public IndexModel(INotificationService notificationService, ApplicationDbContext context)
        {
            _notificationService = notificationService;
            _context = context;
        }

        public IEnumerable<NotificationViewModel> Notifications { get; set; } = new List<NotificationViewModel>();
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
                    Notifications = _notificationService.GetNotificationsByMember(sessionMemberId.Value);
                }
            }
            else
            {
                // Admin can select which member's notifications to view
                Members = await _context.Members.ToListAsync();

                if (memberId.HasValue && memberId.Value > 0)
                {
                    SelectedMemberId = memberId.Value;
                    Notifications = _notificationService.GetNotificationsByMember(memberId.Value);
                }
            }

            return Page();
        }
    }
}
