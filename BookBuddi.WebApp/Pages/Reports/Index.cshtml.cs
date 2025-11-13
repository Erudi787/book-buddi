using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookBuddi.Pages.Reports
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Immediately redirect to the Borrowing report when /Reports is requested
            return RedirectToPage("/Reports/Borrowing");
        }
    }
}