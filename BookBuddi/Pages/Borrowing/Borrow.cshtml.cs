using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;

namespace BookBuddi.Pages.Borrowing
{
    public class BorrowModel : PageModel
    {
        private readonly BorrowingService _borrowingService;
        private readonly MemberService _memberService;
        private readonly BookService _bookService;

        public BorrowModel(BorrowingService borrowingService, MemberService memberService, BookService bookService)
        {
            _borrowingService = borrowingService;
            _memberService = memberService;
            _bookService = bookService;
        }

        public List<Member> Members { get; set; } = new List<Member>();
        public List<Book> AvailableBooks { get; set; } = new List<Book>();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            Members = (await _memberService.GetActiveMembersAsync()).ToList();
            AvailableBooks = (await _bookService.GetAvailableBooksAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(int memberId, int bookId, int borrowDays)
        {
            try
            {
                await _borrowingService.BorrowBookAsync(memberId, bookId, borrowDays);
                SuccessMessage = "Book borrowed successfully!";

                // Reload dropdowns
                Members = (await _memberService.GetActiveMembersAsync()).ToList();
                AvailableBooks = (await _bookService.GetAvailableBooksAsync()).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                // Reload dropdowns
                Members = (await _memberService.GetActiveMembersAsync()).ToList();
                AvailableBooks = (await _bookService.GetAvailableBooksAsync()).ToList();

                return Page();
            }
        }
    }
}
