using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.BookRequests
{
    public class IndexModel : PageModel
    {
        private readonly IBookRequestService _requestService;
        private readonly IMemberService _memberService;

        public IndexModel(IBookRequestService requestService, IMemberService memberService)
        {
            _requestService = requestService;
            _memberService = memberService;
        }

        public List<RequestWithMemberViewModel> Requests { get; set; } = new List<RequestWithMemberViewModel>();

        public void OnGet()
        {
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
