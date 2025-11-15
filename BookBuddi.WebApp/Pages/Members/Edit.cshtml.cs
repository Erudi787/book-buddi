using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Pages.Members
{
    public class EditModel : PageModel
    {
        private readonly IMemberService _memberService;

        public EditModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [BindProperty]
        public MemberViewModel Member { get; set; } = new MemberViewModel();

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? ConfirmPassword { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            var member = _memberService.GetMemberById(id);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToPage("./Index");
            }

            Member = member;
            return Page();
        }

        public IActionResult OnPost()
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
                ErrorMessage = "Please correct the validation errors.";
                return Page();
            }

            try
            {
                // If password fields are provided, validate and update password
                if (!string.IsNullOrWhiteSpace(NewPassword))
                {
                    if (NewPassword != ConfirmPassword)
                    {
                        ErrorMessage = "Passwords do not match.";
                        return Page();
                    }

                    var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                    _memberService.ChangePassword(Member.MemberId, NewPassword, adminName);
                }

                // Update member details
                var adminUpdater = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _memberService.UpdateMember(Member, adminUpdater);

                TempData["SuccessMessage"] = $"Member {Member.FullName} updated successfully.";
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
