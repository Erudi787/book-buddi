using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BookBuddi.Pages.Account
{
    public class EnterVerificationCodeModel : PageModel
    {
        private readonly IMemberService _memberService;

        public EnterVerificationCodeModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Verification code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must contain only digits")]
        public string VerificationCode { get; set; } = string.Empty;

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToPage("/Account/Register");
            }

            Email = email;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please enter a valid 6-digit verification code.";
                return Page();
            }

            try
            {
                // Find member by verification code
                var member = _memberService.GetMemberByVerificationCode(VerificationCode);

                if (member == null)
                {
                    ErrorMessage = "Invalid verification code. Please check and try again.";
                    return Page();
                }

                // Verify the email matches
                if (!member.Email.Equals(Email, StringComparison.OrdinalIgnoreCase))
                {
                    ErrorMessage = "Verification code does not match the email address.";
                    return Page();
                }

                // Check if token has expired
                if (member.EmailVerificationTokenExpiry.HasValue &&
                    member.EmailVerificationTokenExpiry.Value < DateTime.Now)
                {
                    ErrorMessage = "This verification code has expired. Please request a new one.";
                    return Page();
                }

                // Check if already verified
                if (member.EmailVerified)
                {
                    SuccessMessage = "Your email is already verified. You can proceed to login.";
                    return Page();
                }

                // Verify the email
                member.EmailVerified = true;
                member.EmailVerificationToken = null;
                member.EmailVerificationCode = null;
                member.EmailVerificationTokenExpiry = null;
                member.UpdatedBy = "Email Verification";
                member.UpdatedTime = DateTime.Now;

                _memberService.UpdateMember(member, "Email Verification");
                SuccessMessage = "Your email has been successfully verified! You can now login to your account.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Verification failed: {ex.Message}";
                Console.WriteLine($"Email verification error: {ex}");
                return Page();
            }
        }
    }
}
