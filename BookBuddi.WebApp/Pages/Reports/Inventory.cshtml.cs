using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Data.Models;

namespace BookBuddi.WebApp.Pages
{
    public class InventoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public InventoryModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // Inventory Data
        public List<BookInventoryDTO> BookList { get; set; } = new();
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int CurrentlyBorrowed { get; set; }
        public int ReservedBooks { get; set; }
        public int LostBooks { get; set; }

        // Card Highlight
        public string CurrentFilter { get; set; } = "All";

        // Add New Book
        [BindProperty]
        public BookInventoryDTO NewBook { get; set; } = new(); // <- fixes CS8618

        public class BookInventoryDTO
        {
            public string? BookTitle { get; set; }
            public string? Author { get; set; }
            public int Available { get; set; }
            public int Borrowed { get; set; }
        }

        public async Task<IActionResult> OnGet(string filter, string search)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            CurrentFilter = string.IsNullOrEmpty(filter) ? "All" : filter;

            var books = await _db.Books.ToListAsync();
            var transactions = await _db.BorrowTransactions.ToListAsync();
            var requests = await _db.BookRequests.ToListAsync();

            TotalBooks = books.Count;
            AvailableBooks = books.Sum(b => b.AvailableCopies);
            CurrentlyBorrowed = transactions.Count(t => t.ReturnDate == null);
            ReservedBooks = requests.Count(r => r.Status == Resources.Constants.RequestStatus.Pending ||
                                               r.Status == Resources.Constants.RequestStatus.Approved);
            LostBooks = 0;

            BookList = books.Select(b => new BookInventoryDTO
            {
                BookTitle = b.BookTitle,
                Author = null,
                Available = b.AvailableCopies,
                Borrowed = transactions.Count(t => t.BookId == b.BookId && t.ReturnDate == null)
            }).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                BookList = BookList
                    .Where(b => (b.BookTitle ?? "").ToLower().Contains(search))
                    .ToList();
            }

            // Apply filter for cards
            BookList = CurrentFilter switch
            {
                "Available" => BookList.Where(b => b.Available > 0).ToList(),
                "Borrowed" => BookList.Where(b => b.Borrowed > 0).ToList(),
                "Reserved" => BookList.Where(b => b.Available == 0 && b.Borrowed == 0).ToList(),
                "Lost" => new List<BookInventoryDTO>(), // placeholder
                _ => BookList
            };

            return Page();
        }

        // Handle Add New Book form submission
        public async Task<IActionResult> OnPostAddBook()
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            if (!ModelState.IsValid)
                return Page();

            var book = new Book
            {
                BookTitle = NewBook.BookTitle ?? "",
                AvailableCopies = NewBook.Available,
                // Author and other fields can be added when needed
            };

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book added successfully!";

            return RedirectToPage(); // reloads the page
        }
    }
}
