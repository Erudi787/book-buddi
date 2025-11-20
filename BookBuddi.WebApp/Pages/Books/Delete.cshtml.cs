using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Pages.Books
{
    public class DeleteModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly ApplicationDbContext _context;

        public DeleteModel(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        [BindProperty]
        public BookViewModel Book { get; set; } = new BookViewModel();
        public string? ErrorMessage { get; set; }
        public bool HasTransactionHistory { get; set; }

        public IActionResult OnGet(int id)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }

            Book = book;

            // Check if the book has transaction history
            HasTransactionHistory = _context.BorrowTransactions.Any(bt => bt.BookId == id);

            return Page();
        }

        public IActionResult OnPost()
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            try
            {
                // Check if book will be archived or deleted
                var hasTransactions = _context.BorrowTransactions.Any(bt => bt.BookId == Book.BookId);

                _bookService.DeleteBook(Book.BookId);

                // Set success message
                if (hasTransactions)
                {
                    TempData["SuccessMessage"] = "Book has been archived successfully. It is now hidden from the catalog.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Book has been permanently deleted from the system.";
                }

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
