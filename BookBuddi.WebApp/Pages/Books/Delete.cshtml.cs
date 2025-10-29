using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Books
{
    public class DeleteModel : PageModel
    {
        private readonly IBookService _bookService;

        public DeleteModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        [BindProperty]
        public BookViewModel Book { get; set; } = new BookViewModel();
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }

            Book = book;
            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                _bookService.DeleteBook(Book.BookId);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                var book = _bookService.GetBookById(Book.BookId);
                if (book != null)
                {
                    Book = book;
                }
                return Page();
            }
        }
    }
}
