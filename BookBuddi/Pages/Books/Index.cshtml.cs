using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;

namespace BookBuddi.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly BookService _bookService;

        public IndexModel(BookService bookService)
        {
            _bookService = bookService;
        }

        public IEnumerable<Book> Books { get; set; } = new List<Book>();
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Books = await _bookService.SearchBooksAsync(searchTerm);
            }
            else
            {
                Books = await _bookService.GetAllBooksAsync();
            }
        }
    }
}
