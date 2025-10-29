using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Authors
{
    public class EditModel : PageModel
    {
        private readonly IAuthorService _authorService;

        public EditModel(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [BindProperty]
        public AuthorViewModel Author { get; set; } = new AuthorViewModel();
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            var author = _authorService.GetAuthorById(id);
            if (author == null) return NotFound();
            Author = author;
            return Page();
        }

        public IActionResult OnPost()
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin) return RedirectToPage("/Admin/Login");

            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _authorService.UpdateAuthor(Author, adminName);
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
