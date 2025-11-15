using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookBuddi.WebApp.Pages.Admin
{
    public class AccessDeniedModel : PageModel
    {
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Get error message from TempData if available
            ErrorMessage = TempData["ErrorMessage"] as string;
        }
    }
}
