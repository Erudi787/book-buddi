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

        public void OnGet()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var memberId = HttpContext.Session.GetInt32("MemberId");
            
            // Load notifications for members
            if (userRole == "Member" && memberId.HasValue)
            {
                var allNotifications = _notificationService.GetNotificationsByMember(memberId.Value);
                RecentNotifications = allNotifications.OrderByDescending(n => n.DateCreated).Take(5);
                UnreadNotificationCount = allNotifications.Count(n => !n.IsRead);
            }
            
            var requestList = _requestService.GetAllRequests();

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
        }

        public class RequestWithMemberViewModel
        {
            public BookRequestViewModel Request { get; set; } = new BookRequestViewModel();
            public string MemberName { get; set; } = string.Empty;
        }
    }
}
