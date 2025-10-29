using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;

namespace BookBuddi.Pages.Notifications
{
    public class MarkAllReadModel : PageModel
    {
        private readonly INotificationService _notificationService;

        public MarkAllReadModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult OnGet(int memberId)
        {
            _notificationService.MarkAllAsRead(memberId);
            return RedirectToPage("./Index", new { memberId });
        }
    }
}
