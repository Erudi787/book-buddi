using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Pages.BookRequests
{
    public class RejectModel : PageModel
    {
        private readonly IBookRequestService _requestService;

        public RejectModel(IBookRequestService requestService)
        {
            _requestService = requestService;
        }

        public IActionResult OnGet(int id, string? returnUrl)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
            _requestService.RejectRequest(id, "Rejected by admin", adminName);

            // Redirect back to the page that initiated the action
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToPage("./Index");
        }
    }
}
