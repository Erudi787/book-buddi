using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Interfaces;

namespace BookBuddi.Pages.Authors
{
    public class EditModel : PageModel
    {
        private readonly IAuthorRepository _authorRepository;

        public EditModel(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [BindProperty]
        public Author Author { get; set; } = new Author();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            if (author == null) return NotFound();
            Author = author;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin) return RedirectToPage("/Admin/Login");

            try
            {
                await _authorRepository.UpdateAuthorAsync(Author);
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
