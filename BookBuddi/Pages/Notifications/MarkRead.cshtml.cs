using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services;

namespace BookBuddi.Pages.Notifications
{
    public class MarkReadModel : PageModel
    {
        private readonly NotificationService _notificationService;

        public MarkReadModel(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IActionResult> OnGetAsync(int id, int memberId)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToPage("./Index", new { memberId });
        }
    }
}
