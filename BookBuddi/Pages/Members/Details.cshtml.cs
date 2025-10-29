using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;

namespace BookBuddi.Pages.Members
{
    public class DetailsModel : PageModel
    {
        private readonly MemberService _memberService;

        public DetailsModel(MemberService memberService)
        {
            _memberService = memberService;
        }

        public Member? Member { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Member = await _memberService.GetMemberByIdAsync(id);

            if (Member == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
