using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services;

namespace BookBuddi.Pages.Members
{
    public class SuspendModel : PageModel
    {
        private readonly MemberService _memberService;

        public SuspendModel(MemberService memberService)
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

        public async Task<IActionResult> OnPostAsync(int id, string reason)
        {
            try
            {
                await _memberService.SuspendMemberAsync(id, reason);
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
