using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookBuddi.WebApp.Attributes
{
    /// <summary>
    /// Authorization attribute that restricts access to logged-in users (both Member and Admin).
    /// Redirects unauthorized users to the login page.
    /// </summary>
    public class MemberAuthorizeAttribute : Attribute, IPageFilter
    {
        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // No action needed before handler selection
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var userRole = httpContext.Session.GetString("UserRole");

            // Check if user is logged in (either Member or Admin)
            if (string.IsNullOrEmpty(userRole))
            {
                // Redirect to Member/Account login page
                context.Result = new RedirectToPageResult("/Account/Login");
            }
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            // No action needed after handler execution
        }
    }
}
