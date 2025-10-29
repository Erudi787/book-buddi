using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;

namespace BookBuddi.Pages.Books
{
    public class DeleteModel : PageModel
    {
        private readonly BookService _bookService;

        public DeleteModel(BookService bookService)
        {
            _bookService = bookService;
        }

        [BindProperty]
        public Book Book { get; set; } = new Book();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var book = await _bookService.GetBookDetailsAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            Book = book;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _bookService.DeleteBookAsync(Book.BookId);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                var book = await _bookService.GetBookDetailsAsync(Book.BookId);
                if (book != null)
                {
                    Book = book;
                }
                return Page();
            }
        }
    }
}
