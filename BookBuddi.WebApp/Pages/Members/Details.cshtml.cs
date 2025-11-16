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
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            Member = _memberService.GetMemberById(id);

            if (Member == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
