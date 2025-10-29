using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;

namespace BookBuddi.Pages.Notifications
{
    public class MarkReadModel : PageModel
    {
        private readonly INotificationService _notificationService;

        public MarkReadModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult OnGet(int id, int memberId)
        {
            _notificationService.MarkAsRead(id);
            return RedirectToPage("./Index", new { memberId });
        }
    }
}
