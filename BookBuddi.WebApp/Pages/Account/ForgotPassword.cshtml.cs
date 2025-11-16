using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;

namespace BookBuddi.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly IConfiguration _configuration;

        public ForgotPasswordModel(IMemberService memberService, IConfiguration configuration)
        {
            _memberService = memberService;
            _configuration = configuration;
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

                // In a real application, you would send an email with the reset link
                // For now, we'll show the link in the success message (development only)
                var resetUrl = Url.Page("/Account/ResetPassword", null, new { token }, Request.Scheme);

                // TODO: Replace with actual email sending service
                // await _emailService.SendPasswordResetEmail(email, resetUrl);

                SuccessMessage = $"If an account exists with that email, a password reset link has been sent. " +
                                $"(DEV MODE: {resetUrl})";

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
