using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services;

namespace BookBuddi.Pages.Members
{
    public class ReactivateModel : PageModel
    {
        private readonly MemberService _memberService;

        public ReactivateModel(MemberService memberService)
        {
            _memberService = memberService;
        }

        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            MemberId = id;
            MemberName = $"{member.FirstName} {member.LastName}";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _memberService.ReactivateMemberAsync(id);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                var member = await _memberService.GetMemberByIdAsync(id);
                if (member != null)
                {
                    MemberId = id;
                    MemberName = $"{member.FirstName} {member.LastName}";
                }
                return Page();
            }
        }
    }
}
