using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;

namespace BookBuddi.Pages.Members
{
    public class IndexModel : PageModel
    {
        private readonly MemberService _memberService;

        public IndexModel(MemberService memberService)
        {
            _memberService = memberService;
        }

        public IEnumerable<Member> Members { get; set; } = new List<Member>();
        public string? SearchTerm { get; set; }

        public async Task<IActionResult> OnGetAsync(string? searchTerm)
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
                Members = await _memberService.SearchMembersAsync(searchTerm);
            }
            else
            {
                Members = await _memberService.GetAllMembersAsync();
            }

            return Page();
        }
    }
}
