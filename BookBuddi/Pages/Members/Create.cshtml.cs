using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;

namespace BookBuddi.Pages.Members
{
    public class CreateModel : PageModel
    {
        private readonly MemberService _memberService;

        public CreateModel(MemberService memberService)
        {
            _memberService = memberService;
        }

        [BindProperty]
        public Member Member { get; set; } = new Member();
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _memberService.RegisterMemberAsync(Member);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
