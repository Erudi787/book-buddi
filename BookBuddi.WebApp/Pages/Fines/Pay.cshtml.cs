using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Fines
{
    public class PayModel : PageModel
    {
        private readonly IFineService _fineService;
        private readonly IMemberService _memberService;
        private readonly INotificationService _notificationService;

        public PayModel(IFineService fineService, IMemberService memberService, INotificationService notificationService)
        {
            _fineService = fineService;
            _memberService = memberService;
            _notificationService = notificationService;
        }

        public FineViewModel? Fine { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public List<NotificationViewModel> RecentNotifications { get; set; } = new();
        public int UnreadNotificationCount { get; set; }

        public IActionResult OnGet(int id)
        {
            Fine = _fineService.GetFineById(id);

            if (Fine == null)
            {
                return NotFound();
            }

            // Get member name
            var member = _memberService.GetMemberById(Fine.MemberId);
            if (member != null)
            {
                MemberName = $"{member.FirstName} {member.LastName}";
            }

            // Load notifications for members
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Member")
            {
                var memberId = HttpContext.Session.GetInt32("MemberId");
                if (memberId.HasValue)
                {
                    var allNotifications = _notificationService.GetNotificationsByMember(memberId.Value);
                    RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5).ToList();
                    UnreadNotificationCount = _notificationService.GetUnreadCount(memberId.Value);
                }
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            try
            {
                var updatedBy = HttpContext.Session.GetString("MemberName") ?? HttpContext.Session.GetString("AdminName") ?? "System";
                _fineService.PayFine(id, updatedBy);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Fine = _fineService.GetFineById(id);
                
                // Reload notifications for members
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole == "Member")
                {
                    var memberId = HttpContext.Session.GetInt32("MemberId");
                    if (memberId.HasValue)
                    {
                        var allNotifications = _notificationService.GetNotificationsByMember(memberId.Value);
                        RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5).ToList();
                        UnreadNotificationCount = _notificationService.GetUnreadCount(memberId.Value);
                    }
                }
                
                return Page();
            }
        }
    }
}
