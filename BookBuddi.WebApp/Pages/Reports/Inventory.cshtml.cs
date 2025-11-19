using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Data.Models;

namespace BookBuddi.Pages.Reports
{
    public class InventoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public InventoryModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<InventoryDTO> BookList { get; set; } = new();

        // Summary counts
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int CurrentlyBorrowed { get; set; }
        public int ReservedBooks { get; set; }

        // Current filter
        public string CurrentFilter { get; set; } = "All";

        public async Task<IActionResult> OnGet(string filter, string search)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
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

            BookList = books.Select(b => new InventoryDTO
            {
                BookTitle = b.BookTitle,
                Available = b.AvailableCopies,
                Borrowed = transactions.Count(t => t.BookId == b.BookId && t.ReturnDate == null)
            }).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                BookList = BookList.Where(b => (b.BookTitle ?? "").ToLower().Contains(search)).ToList();
            }

            BookList = CurrentFilter switch
            {
                "Available" => BookList.Where(b => b.Available > 0).ToList(),
                "Borrowed" => BookList.Where(b => b.Borrowed > 0).ToList(),
                "Reserved" => BookList.Where(b => b.Available == 0 && b.Borrowed == 0).ToList(),
                _ => BookList
            };

            return Page();
        }

        public class InventoryDTO
        {
            public string? BookTitle { get; set; }
            public int Available { get; set; }
            public int Borrowed { get; set; }
        }
    }
}
