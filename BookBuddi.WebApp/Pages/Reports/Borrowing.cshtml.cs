using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using System.Linq;

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

    public async Task OnGet()
    {
        var today = DateTime.Today;

        // Load all transactions
        var transactions = await _db.BorrowTransactions.ToListAsync();
        var books = await _db.Books.ToListAsync();
        var members = await _db.Members.ToListAsync();

        // Summary calculations
        TotalBorrowed = transactions.Count;
        TotalReturned = transactions.Count(t => t.ReturnDate != null);
        TotalActive = transactions.Count(t => t.ReturnDate == null);
        TotalOverdue = transactions.Count(t => t.ReturnDate == null && t.DueDate < today);

        // Map data into DTO
        BorrowList = transactions
            .Select(t => new BorrowDTO
            {
                BookTitle = books.FirstOrDefault(b => b.BookId == t.BookId)?.BookTitle ?? "-",
                MemberName = members.FirstOrDefault(m => m.MemberId == t.MemberId)?.FirstName + " " +
                             members.FirstOrDefault(m => m.MemberId == t.MemberId)?.LastName ?? "-",
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