using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;

namespace BookBuddi.Pages.Members
{
    public class ReactivateModel : PageModel
    {
        private readonly IMemberService _memberService;

        public ReactivateModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            var member = _memberService.GetMemberById(id);
            if (member == null)
            {
                return NotFound();
            }

            MemberId = id;
            MemberName = $"{member.FirstName} {member.LastName}";
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            try
            {
                var member = _memberService.GetMemberById(id);
                if (member == null)
                {
                    return NotFound();
                }

                // Update member status to Active
                member.Status = BookBuddi.Resources.Constants.MemberStatus.Active;

                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _memberService.UpdateMember(member, adminName);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                var member = _memberService.GetMemberById(id);
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
