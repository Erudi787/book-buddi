using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Models;
using BookBuddi.Services;
using BookBuddi.Data;

namespace BookBuddi.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly BookService _bookService;
        private readonly ApplicationDbContext _context;

        public CreateModel(BookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = new Book();

        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDropdownsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            try
            {
                await _bookService.AddBookAsync(Book);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await LoadDropdownsAsync();
                return Page();
            }
        }

        private async Task LoadDropdownsAsync()
        {
            Categories = await _context.Categories.ToListAsync();
            Genres = await _context.Genres.ToListAsync();
        }
    }
}
