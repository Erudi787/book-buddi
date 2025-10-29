using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Members
{
    public class DetailsModel : PageModel
    {
        private readonly IMemberService _memberService;

        public DetailsModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public MemberViewModel? Member { get; set; }

        public IActionResult OnGet(int id)
        {
            Member = _memberService.GetMemberById(id);

            if (Member == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
