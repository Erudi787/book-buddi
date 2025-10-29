using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using AdminModel = BookBuddi.Data.Models.Admin;

namespace BookBuddi.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AdminModel> _signInManager;

        public LogoutModel(SignInManager<AdminModel> signInManager)
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
