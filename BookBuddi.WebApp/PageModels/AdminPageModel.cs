using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookBuddi.WebApp.PageModels
{
    /// <summary>
    /// Base PageModel for admin-only pages. Provides built-in authorization checking.
    /// All pages that inherit from this will automatically require Admin role.
    /// </summary>
    public abstract class AdminPageModel : PageModel
    {
        /// <summary>
        /// Checks if the current user has Admin role authorization.
        /// Call this at the beginning of OnGet/OnPost methods.
        /// </summary>
        /// <returns>RedirectToPageResult to login if unauthorized, null if authorized</returns>
        protected IActionResult? CheckAdminAuthorization()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage("/Account/Login");
            }

            return null;
        }

        /// <summary>
        /// Gets the admin name from session
        /// </summary>
        protected string? GetAdminName()
        {
            return HttpContext.Session.GetString("AdminName");
        }

        /// <summary>
        /// Gets the admin email from session
        /// </summary>
        protected string? GetAdminEmail()
        {
            return HttpContext.Session.GetString("AdminEmail");
        }
    }
}
