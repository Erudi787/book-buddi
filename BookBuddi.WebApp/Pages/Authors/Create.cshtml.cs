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

        public void OnGet()
        {
        }

        public IActionResult OnPost(string firstName, string lastName, string? biography)
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin)
            {
                return RedirectToPage("/Admin/Login");
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
