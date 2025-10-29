using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services;

namespace BookBuddi.Pages.Notifications
{
    public class MarkAllReadModel : PageModel
    {
        private readonly NotificationService _notificationService;

        public MarkAllReadModel(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IActionResult> OnGetAsync(int memberId)
        {
            await _notificationService.MarkAllAsReadAsync(memberId);
            return RedirectToPage("./Index", new { memberId });
        }
    }
}
