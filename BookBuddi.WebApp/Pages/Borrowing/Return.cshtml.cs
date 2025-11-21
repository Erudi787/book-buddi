using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Pages.Borrowing
{
    public class ReturnModel : PageModel
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;
        private readonly INotificationService _notificationService;

        public ReturnModel(IBorrowingService borrowingService, IBookService bookService, IMemberService memberService, INotificationService notificationService)
        {
            _borrowingService = borrowingService;
            _bookService = bookService;
            _memberService = memberService;
            _notificationService = notificationService;
        }

        public BorrowTransactionViewModel? Transaction { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadNotificationCount { get; set; }

        // New properties for admin transaction selection mode
        public bool IsSelectionMode { get; set; } = false;
        public List<MemberViewModel> Members { get; set; } = new List<MemberViewModel>();
        public List<TransactionWithDetails> ActiveTransactions { get; set; } = new List<TransactionWithDetails>();
        public int? SelectedMemberId { get; set; }

        public class TransactionWithDetails
        {
            public BorrowTransactionViewModel Transaction { get; set; } = null!;
            public string BookTitle { get; set; } = string.Empty;
            public string MemberName { get; set; } = string.Empty;
            public bool IsOverdue { get; set; }
            public int DaysOverdue { get; set; }
        }

        public IActionResult OnGet(int? id, int? memberId)
        {
            // Authentication check - both Admin and Member can access
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToPage("/Account/Login");
            }

            // If no ID provided, show selection mode (primarily for Admin)
            if (!id.HasValue || id.Value == 0)
            {
                IsSelectionMode = true;

                if (userRole == "Admin")
                {
                    // Load members for dropdown
                    Members = _memberService.GetMembersByStatus(MemberStatus.Active).ToList();
                    SelectedMemberId = memberId;

                    // Load active transactions (optionally filtered by member)
                    var allTransactions = _borrowingService.GetAllTransactions()
                        .Where(t => t.Status == TransactionStatus.Active || t.Status == TransactionStatus.Overdue);

                    if (memberId.HasValue && memberId.Value > 0)
                    {
                        allTransactions = allTransactions.Where(t => t.MemberId == memberId.Value);
                    }

                    ActiveTransactions = allTransactions.Select(t => new TransactionWithDetails
                    {
                        Transaction = t,
                        BookTitle = _bookService.GetBookById(t.BookId)?.BookTitle ?? "Unknown Book",
                        MemberName = GetMemberFullName(t.MemberId),
                        IsOverdue = t.DueDate < DateTime.Now,
                        DaysOverdue = t.DueDate < DateTime.Now ? (DateTime.Now - t.DueDate).Days : 0
                    }).OrderByDescending(t => t.IsOverdue).ThenBy(t => t.Transaction.DueDate).ToList();
                }
                else
                {
                    // Members should not access this page without a transaction ID
                    TempData["ErrorMessage"] = "Please select a book to return from your borrowing list.";
                    return RedirectToPage("/Borrowing/Index");
                }

                return Page();
            }

            // Existing logic for when ID is provided (confirmation mode)
            IsSelectionMode = false;
            Transaction = _borrowingService.GetTransactionById(id.Value);

            if (Transaction == null)
            {
                return NotFound();
            }

            // Authorization check for Members - they can only return their own books
            if (userRole == "Member")
            {
                var sessionMemberId = HttpContext.Session.GetInt32("MemberId");
                if (!sessionMemberId.HasValue || Transaction.MemberId != sessionMemberId.Value)
                {
                    TempData["ErrorMessage"] = "You can only return books that you have borrowed.";
                    return RedirectToPage("/Borrowing/Index");
                }
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
            // Authentication check - both Admin and Member can access
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToPage("/Account/Login");
            }

            // Authorization check for Members - they can only return their own books
            if (userRole == "Member")
            {
                var transaction = _borrowingService.GetTransactionById(id);
                var memberId = HttpContext.Session.GetInt32("MemberId");

                if (transaction == null)
                {
                    return NotFound();
                }

                if (!memberId.HasValue || transaction.MemberId != memberId.Value)
                {
                    TempData["ErrorMessage"] = "You can only return books that you have borrowed.";
                    return RedirectToPage("/Borrowing/Index");
                }
            }

            try
            {
                var processedBy = userRole == "Admin"
                    ? HttpContext.Session.GetString("AdminName") ?? "Admin"
                    : HttpContext.Session.GetString("MemberName") ?? "Member";
                _borrowingService.ReturnBook(id, processedBy);

                if (userRole == "Admin")
                {
                    return RedirectToPage("/Admin/Index");
                }
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Transaction = _borrowingService.GetTransactionById(id);
                return Page();
            }
        }

        private string GetMemberFullName(int memberId)
        {
            var member = _memberService.GetMemberById(memberId);
            return member != null ? $"{member.FirstName} {member.LastName}" : "Unknown Member";
        }
    }
}
