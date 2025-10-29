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

        public IEnumerable<MemberViewModel> Members { get; set; } = new List<MemberViewModel>();
        public string? SearchTerm { get; set; }

        public IActionResult OnGet(string? searchTerm)
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Index");
            }

            SearchTerm = searchTerm;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Members = _memberService.SearchMembers(searchTerm);
            }
            else
            {
                Members = _memberService.GetAllMembers();
            }

            return Page();
        }
    }
}
