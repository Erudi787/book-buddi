using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Members
{
    public class CreateModel : PageModel
    {
        private readonly IMemberService _memberService;

        public CreateModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [BindProperty]
        public MemberViewModel Member { get; set; } = new MemberViewModel();
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string password)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _memberService.AddMember(Member, password, adminName);
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
