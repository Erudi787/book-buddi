using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using AdminModel = BookBuddi.Data.Models.Admin;

namespace BookBuddi.Pages.Account
{
    public class VerifyEmailModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly UserManager<AdminModel> _userManager;

        public VerifyEmailModel(IMemberService memberService, UserManager<AdminModel> userManager)
        {
            _memberService = memberService;
            _userManager = userManager;
        }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string? token)
        {
            Console.WriteLine($"[VerifyEmail] Received token: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}...");

            if (string.IsNullOrWhiteSpace(token))
            {
                ErrorMessage = "Invalid verification link. Please check your email and try again.";
                return Page();
            }

            try
            {
                // Try to verify member email first
                Console.WriteLine($"[VerifyEmail] Looking up member by token...");
                var member = _memberService.GetMemberByVerificationToken(token);

                if (member != null)
                {
                    Console.WriteLine($"[VerifyEmail] Found member: {member.Email}, Verified: {member.EmailVerified}, Expiry: {member.EmailVerificationTokenExpiry}");

                    // Check if token has expired
                    if (member.EmailVerificationTokenExpiry.HasValue &&
                        member.EmailVerificationTokenExpiry.Value < DateTime.Now)
                    {
                        ErrorMessage = "This verification link has expired. Please request a new verification email.";
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

                // If not a member, try admin verification using Identity tokens
                // This would require additional implementation with UserManager
                // For now, we'll just show an error
                Console.WriteLine($"[VerifyEmail] No member found with token. Token might be invalid or already used.");
                ErrorMessage = "Invalid or expired verification token. Please try registering again or contact support.";
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
