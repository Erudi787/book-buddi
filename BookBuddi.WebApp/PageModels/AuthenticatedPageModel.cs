using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookBuddi.WebApp.PageModels
{
    /// <summary>
    /// Base PageModel for pages requiring authentication (Member or Admin).
    /// All pages that inherit from this will require users to be logged in.
    /// </summary>
    public abstract class AuthenticatedPageModel : PageModel
    {
        /// <summary>
        /// Checks if the current user is authenticated (Member or Admin).
        /// Call this at the beginning of OnGet/OnPost methods.
        /// </summary>
        /// <returns>RedirectToPageResult to login if not authenticated, null if authenticated</returns>
        protected IActionResult? CheckAuthentication()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToPage("/Account/Login");
            }

            return null;
        }

        /// <summary>
        /// Checks if the current user is an Admin
        /// </summary>
        protected bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        /// <summary>
        /// Checks if the current user is a Member
        /// </summary>
        protected bool IsMember()
        {
            return HttpContext.Session.GetString("UserRole") == "Member";
        }

        /// <summary>
        /// Gets the current user's role
        /// </summary>
        protected string? GetUserRole()
        {
            return HttpContext.Session.GetString("UserRole");
        }

        /// <summary>
        /// Gets the member ID from session (returns null if user is Admin)
        /// </summary>
        protected int? GetMemberId()
        {
            return HttpContext.Session.GetInt32("MemberId");
        }

        /// <summary>
        /// Gets the member name from session
        /// </summary>
        protected string? GetMemberName()
        {
            return HttpContext.Session.GetString("MemberName");
        }
    }
}
