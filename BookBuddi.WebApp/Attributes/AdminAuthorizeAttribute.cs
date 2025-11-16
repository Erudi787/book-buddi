using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookBuddi.WebApp.Attributes
{
    /// <summary>
    /// Authorization attribute that restricts access to Admin users only.
    /// Redirects unauthorized users to the login page.
    /// </summary>
    public class AdminAuthorizeAttribute : Attribute, IPageFilter
    {
        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // No action needed before handler selection
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var userRole = httpContext.Session.GetString("UserRole");

            // Check if user is logged in as Admin
            if (userRole != "Admin")
            {
                // Redirect to Admin login page
                context.Result = new RedirectToPageResult("/Account/Login");
            }
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            // No action needed after handler execution
        }
    }
}
