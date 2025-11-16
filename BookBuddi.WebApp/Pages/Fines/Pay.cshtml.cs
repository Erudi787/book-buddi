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
            // Authentication check - both Admin and Member can access
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToPage("/Account/Login");
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
            // Authentication check - both Admin and Member can access
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var updatedBy = userRole == "Admin"
                    ? HttpContext.Session.GetString("AdminName") ?? "Admin"
                    : HttpContext.Session.GetString("MemberName") ?? "Member";
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
