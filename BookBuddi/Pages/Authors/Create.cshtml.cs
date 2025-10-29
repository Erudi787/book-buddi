using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Interfaces;

namespace BookBuddi.Pages.Authors
{
    public class CreateModel : PageModel
    {
        private readonly IAuthorRepository _authorRepository;

        public CreateModel(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string firstName, string lastName, string? biography)
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin)
            {
                return RedirectToPage("/Admin/Login");
            }

            try
            {
                var author = new Author
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Biography = biography
                };

                await _authorRepository.AddAuthorAsync(author);
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
