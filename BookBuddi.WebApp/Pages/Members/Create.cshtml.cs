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

        public IActionResult OnGet()
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            return Page();
        }

        public IActionResult OnPost(string password)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

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
