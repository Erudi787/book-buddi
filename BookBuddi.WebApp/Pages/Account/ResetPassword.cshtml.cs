using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;

namespace BookBuddi.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IMemberService _memberService;

        public ResetPasswordModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Token { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(Token))
            {
                ErrorMessage = "Invalid password reset link.";
                return Page();
            }

            // Validate token
            if (!_memberService.ValidatePasswordResetToken(Token))
            {
                ErrorMessage = "This password reset link is invalid or has expired.";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string token, string newPassword, string confirmPassword)
        {
            try
            {
                Token = token;

                if (string.IsNullOrEmpty(token))
                {
                    ErrorMessage = "Invalid password reset link.";
                    return Page();
                }

                if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    ErrorMessage = "Please enter and confirm your new password.";
                    return Page();
                }

                if (newPassword != confirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    return Page();
                }

                // Reset password
                _memberService.ResetPassword(token, newPassword);

                SuccessMessage = "Your password has been reset successfully! You can now login with your new password.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
