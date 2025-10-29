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
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin) return RedirectToPage("/Admin/Login");

            _authorService.DeleteAuthor(id);
            return RedirectToPage("./Index");
        }
    }
}
