using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;

namespace BookBuddi.Pages.Authors
{
    public class DeleteModel : PageModel
    {
        private readonly IAuthorService _authorService;

        public DeleteModel(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        public IActionResult OnGet(int id)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            _authorService.DeleteAuthor(id);
            return RedirectToPage("./Index");
        }
    }
}
