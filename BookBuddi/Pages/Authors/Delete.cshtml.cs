using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Interfaces;

namespace BookBuddi.Pages.Authors
{
    public class DeleteModel : PageModel
    {
        private readonly IAuthorRepository _authorRepository;

        public DeleteModel(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin) return RedirectToPage("/Admin/Login");

            await _authorRepository.DeleteAuthorAsync(id);
            return RedirectToPage("./Index");
        }
    }
}
