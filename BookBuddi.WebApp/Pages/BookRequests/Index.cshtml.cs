using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.BookRequests
{
    public class IndexModel : PageModel
    {
        private readonly IBookRequestService _requestService;
        private readonly IMemberService _memberService;
        private readonly INotificationService _notificationService;

        public IndexModel(IBookRequestService requestService, IMemberService memberService, INotificationService notificationService)
        {
            _requestService = requestService;
            _memberService = memberService;
            _notificationService = notificationService;
        }

        public List<RequestWithMemberViewModel> Requests { get; set; } = new List<RequestWithMemberViewModel>();
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadNotificationCount { get; set; }

        public IActionResult OnGet()
        {
            // Authentication check - require login
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
            {
                TempData["ErrorMessage"] = "You must be logged in to access this page.";
                return RedirectToPage("/Account/Login");
            }

            var memberId = HttpContext.Session.GetInt32("MemberId");

            // Load notifications for members
            if (userRole == "Member" && memberId.HasValue)
            {
                var allNotifications = _notificationService.GetNotificationsByMember(memberId.Value);
                RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);
            }

            // Admin sees all requests, Members see only their own
            var requestList = userRole == "Admin"
                ? _requestService.GetAllRequests()
                : (memberId.HasValue ? _requestService.GetRequestsByMember(memberId.Value) : new List<BookRequestViewModel>());

            // Enrich requests with member names
            Requests = requestList.Select(r =>
            {
                var member = _memberService.GetMemberById(r.MemberId);
                return new RequestWithMemberViewModel
                {
                    Request = r,
                    MemberName = member != null ? $"{member.FirstName} {member.LastName}" : "Unknown"
                };
            }).ToList();

            return Page();
        }

        public class RequestWithMemberViewModel
        {
            public BookRequestViewModel Request { get; set; } = new BookRequestViewModel();
            public string MemberName { get; set; } = string.Empty;
        }
    }
}
