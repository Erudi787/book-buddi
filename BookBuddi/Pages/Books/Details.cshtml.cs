using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;

namespace BookBuddi.Pages.Books
{
    public class DetailsModel : PageModel
    {
        private readonly BookService _bookService;

        public DetailsModel(BookService bookService)
        {
            _bookService = bookService;
        }

        public Book? Book { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Book = await _bookService.GetBookDetailsAsync(id);

            if (Book == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
