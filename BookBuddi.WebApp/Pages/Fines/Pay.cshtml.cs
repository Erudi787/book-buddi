using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Fines
{
    public class PayModel : PageModel
    {
        private readonly IFineService _fineService;
        private readonly IMemberService _memberService;

        public PayModel(IFineService fineService, IMemberService memberService)
        {
            _fineService = fineService;
            _memberService = memberService;
        }

        public FineViewModel? Fine { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            Fine = _fineService.GetFineById(id);

            if (Fine == null)
            {
                return NotFound();
            }

            // Get member name
            var member = _memberService.GetMemberById(Fine.MemberId);
            if (member != null)
            {
                MemberName = $"{member.FirstName} {member.LastName}";
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            try
            {
                var updatedBy = HttpContext.Session.GetString("MemberName") ?? HttpContext.Session.GetString("AdminName") ?? "System";
                _fineService.PayFine(id, updatedBy);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Fine = _fineService.GetFineById(id);
                return Page();
            }
        }
    }
}
