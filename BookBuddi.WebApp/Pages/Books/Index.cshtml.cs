using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;

        public IndexModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        public IEnumerable<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public string? SearchTerm { get; set; }

        public void OnGet(string? searchTerm)
        {
            SearchTerm = searchTerm;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Books = _bookService.SearchBooks(searchTerm);
            }
            else
            {
                Books = _bookService.GetAllBooks();
            }
        }
    }
}
