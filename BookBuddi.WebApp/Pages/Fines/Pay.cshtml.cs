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
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

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

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            try
            {
                var updatedBy = HttpContext.Session.GetString("AdminName") ?? "System";
                _fineService.PayFine(id, updatedBy);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Fine = _fineService.GetFineById(id);

                return Page();
            }
        }
    }
}
