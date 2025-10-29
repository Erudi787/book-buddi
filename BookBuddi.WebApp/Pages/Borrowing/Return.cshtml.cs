using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Borrowing
{
    public class ReturnModel : PageModel
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;

        public ReturnModel(IBorrowingService borrowingService, IBookService bookService, IMemberService memberService)
        {
            _borrowingService = borrowingService;
            _bookService = bookService;
            _memberService = memberService;
        }

        public BorrowTransactionViewModel? Transaction { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            Transaction = _borrowingService.GetTransactionById(id);

            if (Transaction == null)
            {
                return NotFound();
            }

            // Get book title
            var book = _bookService.GetBookById(Transaction.BookId);
            if (book != null)
            {
                BookTitle = book.BookTitle;
            }

            // Get member name
            var member = _memberService.GetMemberById(Transaction.MemberId);
            if (member != null)
            {
                MemberName = $"{member.FirstName} {member.LastName}";
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "System";
                _borrowingService.ReturnBook(id, adminName);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Transaction = _borrowingService.GetTransactionById(id);
                return Page();
            }
        }
    }
}
