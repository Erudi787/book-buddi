using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace BookBuddi.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<Models.Admin> _signInManager;

        public LogoutModel(SignInManager<Models.Admin> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Sign out from Identity (if admin)
            await _signInManager.SignOutAsync();

            // Clear session
            HttpContext.Session.Clear();

            return RedirectToPage("/Index");
        }
    }
}
