using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Borrowing
{
    public class BorrowModel : PageModel
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IMemberService _memberService;
        private readonly IBookService _bookService;

        public BorrowModel(IBorrowingService borrowingService, IMemberService memberService, IBookService bookService)
        {
            _borrowingService = borrowingService;
            _memberService = memberService;
            _bookService = bookService;
        }

        public List<MemberViewModel> Members { get; set; } = new List<MemberViewModel>();
        public List<BookViewModel> AvailableBooks { get; set; } = new List<BookViewModel>();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            Members = _memberService.GetMembersByStatus(BookBuddi.Resources.Constants.MemberStatus.Active).ToList();
            AvailableBooks = _bookService.GetAvailableBooks().ToList();
        }

        public IActionResult OnPost(int memberId, int bookId)
        {
            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "System";
                _borrowingService.BorrowBook(memberId, bookId, adminName);
                SuccessMessage = "Book borrowed successfully!";

                // Reload dropdowns
                Members = _memberService.GetMembersByStatus(BookBuddi.Resources.Constants.MemberStatus.Active).ToList();
                AvailableBooks = _bookService.GetAvailableBooks().ToList();

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                // Reload dropdowns
                Members = _memberService.GetMembersByStatus(BookBuddi.Resources.Constants.MemberStatus.Active).ToList();
                AvailableBooks = _bookService.GetAvailableBooks().ToList();

                return Page();
            }
        }
    }
}
