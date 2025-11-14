using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Members
{
    public class IndexModel : PageModel
    {
        private readonly IMemberService _memberService;

        public IndexModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public PagedResult<MemberViewModel> Members { get; set; } = new PagedResult<MemberViewModel>();
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public IActionResult OnGet(string? searchTerm, int pageNumber = 1, int pageSize = 20)
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Index");
            }

            SearchTerm = searchTerm;
            PageNumber = pageNumber;
            PageSize = pageSize;

            Members = _memberService.GetMembersPaged(pageNumber, pageSize, searchTerm);

            return Page();
        }
    }
}
