using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Data.Models;

namespace BookBuddi.Pages.Reports
{
    public class BorrowingModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public BorrowingModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<BorrowDTO> BorrowList { get; set; } = new();

        // Summary counts
        public int TotalBorrowed { get; set; }
        public int TotalReturned { get; set; }
        public int TotalActive { get; set; }
        public int TotalOverdue { get; set; }

        // Current filter
        public string CurrentFilter { get; set; } = "All";

        public async Task<IActionResult> OnGet(string filter, string search)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage("/Account/Login");
            }

            CurrentFilter = string.IsNullOrEmpty(filter) ? "All" : filter;

            var today = DateTime.Today;

            var transactions = await _db.BorrowTransactions.ToListAsync();
            var books = await _db.Books.ToListAsync();
            var members = await _db.Members.ToListAsync();

            var list = transactions.Select(t => new BorrowDTO
            {
                BookTitle = books.FirstOrDefault(b => b.BookId == t.BookId)?.BookTitle ?? "-",
                MemberName = (members.FirstOrDefault(m => m.MemberId == t.MemberId)?.FirstName ?? "") + " " +
                             (members.FirstOrDefault(m => m.MemberId == t.MemberId)?.LastName ?? "-"),
                BorrowDate = t.BorrowDate,
                DueDate = t.DueDate,
                ReturnDate = t.ReturnDate,
                StatusText = t.ReturnDate != null
                    ? "Returned"
                    : (t.DueDate < today ? "Overdue" : "Active"),
                StatusClass = t.ReturnDate != null
                    ? "status-returned"
                    : (t.DueDate < today ? "status-overdue" : "status-active")
            }).ToList();

            TotalBorrowed = list.Count;
            TotalReturned = list.Count(t => t.StatusText == "Returned");
            TotalActive = list.Count(t => t.StatusText == "Active");
            TotalOverdue = list.Count(t => t.StatusText == "Overdue");

            BorrowList = CurrentFilter switch
            {
                "Returned" => list.Where(t => t.StatusText == "Returned").ToList(),
                "Active" => list.Where(t => t.StatusText == "Active").ToList(),
                "Overdue" => list.Where(t => t.StatusText == "Overdue").ToList(),
                _ => list
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                BorrowList = BorrowList
                    .Where(t => (t.BookTitle ?? "").ToLower().Contains(search) ||
                                (t.MemberName ?? "").ToLower().Contains(search))
                    .ToList();
            }

            return Page();
        }

        public class BorrowDTO
        {
            public string? BookTitle { get; set; }
            public string? MemberName { get; set; }
            public DateTime BorrowDate { get; set; }
            public DateTime DueDate { get; set; }
            public DateTime? ReturnDate { get; set; }
            public string? StatusText { get; set; }
            public string? StatusClass { get; set; }
        }
    }
}
