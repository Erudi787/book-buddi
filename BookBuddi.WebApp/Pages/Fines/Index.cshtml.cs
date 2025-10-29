using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Fines
{
    public class IndexModel : PageModel
    {
        private readonly IFineService _fineService;
        private readonly IMemberService _memberService;

        public IndexModel(IFineService fineService, IMemberService memberService)
        {
            _fineService = fineService;
            _memberService = memberService;
        }

        public List<FineWithMemberViewModel> Fines { get; set; } = new List<FineWithMemberViewModel>();

        public IActionResult OnGet()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            // Require login
            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToPage("/Account/Login");
            }

            IEnumerable<FineViewModel> fineList;

            if (userRole == "Member")
            {
                // Show only the member's fines
                var memberId = HttpContext.Session.GetInt32("MemberId");
                if (memberId.HasValue)
                {
                    fineList = _fineService.GetFinesByMember(memberId.Value);
                }
                else
                {
                    fineList = new List<FineViewModel>();
                }
            }
            else
            {
                // Admin sees all fines
                fineList = _fineService.GetAllFines();
            }

            // Enrich fines with member names
            Fines = fineList.Select(f =>
            {
                var member = _memberService.GetMemberById(f.MemberId);
                return new FineWithMemberViewModel
                {
                    Fine = f,
                    MemberName = member != null ? $"{member.FirstName} {member.LastName}" : "Unknown"
                };
            }).ToList();

            return Page();
        }

        public class FineWithMemberViewModel
        {
            public FineViewModel Fine { get; set; } = new FineViewModel();
            public string MemberName { get; set; } = string.Empty;
        }
    }
}
