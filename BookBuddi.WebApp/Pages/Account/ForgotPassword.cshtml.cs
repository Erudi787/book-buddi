using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.Configuration;
using Microsoft.Extensions.Options;

namespace BookBuddi.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly IEmailService _emailService;
        private readonly EmailSettings _emailSettings;

        public ForgotPasswordModel(
            IMemberService memberService,
            IEmailService emailService,
            IOptions<EmailSettings> emailSettings)
        {
            _memberService = memberService;
            _emailService = emailService;
            _emailSettings = emailSettings.Value;
        }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            // Redirect if already logged in
            var userRole = HttpContext.Session.GetString("UserRole");
            if (!string.IsNullOrEmpty(userRole))
            {
                return RedirectToPage(userRole == "Admin" ? "/Admin/Index" : "/Books/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    ErrorMessage = "Please enter your email address.";
                    return Page();
                }

                // Generate reset token
                var token = _memberService.GeneratePasswordResetToken(email);
                var resetUrl = Url.Page("/Account/ResetPassword", null, new { token }, Request.Scheme)!;

                // Get member info for personalized email
                var member = _memberService.GetMemberByEmail(email);
                if (member != null)
                {
                    // Send password reset email
                    try
                    {
                        await _emailService.SendPasswordResetEmailAsync(email, member.FirstName, resetUrl);

                        if (_emailSettings.UseDevelopmentMode)
                        {
                            SuccessMessage = $"Password reset link has been sent to your email. " +
                                           $"(DEV MODE: {resetUrl})";
                        }
                        else
                        {
                            SuccessMessage = "If an account exists with that email, a password reset link has been sent.";
                        }
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine($"Failed to send password reset email: {emailEx.Message}");

                        // In development mode, show the link even if email fails
                        if (_emailSettings.UseDevelopmentMode)
                        {
                            SuccessMessage = $"Email service unavailable. Use this link: {resetUrl}";
                        }
                        else
                        {
                            SuccessMessage = "If an account exists with that email, a password reset link has been sent.";
                        }
                    }
                }
                else
                {
                    // Don't reveal if account exists for security
                    SuccessMessage = "If an account exists with that email, a password reset link has been sent.";
                }

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
