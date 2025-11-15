using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Authors
{
    public class CreateModel : PageModel
    {
        private readonly IAuthorService _authorService;

        public CreateModel(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            return Page();
        }

        public IActionResult OnPost(string firstName, string lastName, string? biography)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            try
            {
                var author = new AuthorViewModel
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Biography = biography
                };

                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _authorService.AddAuthor(author, adminName);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
