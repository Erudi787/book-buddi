using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Data;
using BookBuddi.Data.Models;

namespace BookBuddi.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly ApplicationDbContext _context;

        public EditModel(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        [BindProperty]
        public BookViewModel Book { get; set; } = new BookViewModel();

        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Admin/Login");
            }

            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return RedirectToPage("./Index");
            }

            Book = book;
            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin)
            {
                return RedirectToPage("/Admin/Login");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _bookService.UpdateBook(Book, adminName);
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
