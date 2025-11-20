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
        private readonly IFileUploadService _fileUploadService;
        private readonly ApplicationDbContext _context;

        public EditModel(IBookService bookService, IFileUploadService fileUploadService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _fileUploadService = fileUploadService;
            _context = context;
        }

        [BindProperty]
        public BookViewModel Book { get; set; } = new BookViewModel();

        [BindProperty]
        public IFormFile? CoverImageFile { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
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
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            try
            {
                // Handle file upload if provided
                if (CoverImageFile != null)
                {
                    try
                    {
                        // Delete old cover image if it exists
                        if (!string.IsNullOrEmpty(Book.CoverImageUrl))
                        {
                            await _fileUploadService.DeleteFileAsync(Book.CoverImageUrl);
                        }

                        // Upload new cover image
                        var uploadedPath = await _fileUploadService.UploadBookCoverAsync(CoverImageFile);
                        Book.CoverImageUrl = uploadedPath;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Error uploading cover image: {ex.Message}";
                        await LoadDropdownsAsync();
                        return Page();
                    }
                }

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
