using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Data.Models;
using System.Linq;

namespace BookBuddi.WebApp.Pages
{
    public class InventoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public InventoryModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<BookInventoryDTO> BookList { get; set; } = new();

        // Summary counts
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int CurrentlyBorrowed { get; set; }
        public int ReservedBooks { get; set; }
        public int LostBooks { get; set; }

        public async Task OnGet()
        {
            var books = await _db.Books.ToListAsync();
            var transactions = await _db.BorrowTransactions.ToListAsync();
            var requests = await _db.BookRequests.ToListAsync();

            // Summary calculations
            TotalBooks = books.Count;
            AvailableBooks = books.Sum(b => b.AvailableCopies);
            CurrentlyBorrowed = transactions.Count(t => t.ReturnDate == null);
            ReservedBooks = requests.Count(r => r.Status == BookBuddi.Resources.Constants.RequestStatus.Pending ||
                                               r.Status == BookBuddi.Resources.Constants.RequestStatus.Approved);
            LostBooks = 0;

            // Map data into DTO
            BookList = books.Select(b => new BookInventoryDTO
            {
                BookTitle = b.BookTitle,
                Author = "-",
                Available = b.AvailableCopies,
                Borrowed = transactions.Count(t => t.BookId == b.BookId && t.ReturnDate == null)
            }).ToList();
        }

        public class BookInventoryDTO
        {
            public string? BookTitle { get; set; }
            public string? Author { get; set; }
            public int Available { get; set; }
            public int Borrowed { get; set; }
        }
    }
}