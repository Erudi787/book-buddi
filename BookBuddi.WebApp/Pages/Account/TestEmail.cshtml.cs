using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using System.Security.Cryptography;

namespace BookBuddi.Pages.Account
{
    public class TestEmailModel : PageModel
    {
        private readonly IEmailService _emailService;

        public TestEmailModel(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public string? EmailOutput { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string emailType, string email, string name)
        {
            try
            {
                bool result = false;
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                var verificationCode = new Random().Next(100000, 999999).ToString();

                switch (emailType)
                {
                    case "verification":
                        result = await _emailService.SendMemberVerificationEmailAsync(email, name, token, verificationCode);
                        EmailOutput = $"Verification email sent to: {email}\nName: {name}\nCode: {verificationCode}\nToken: {token.Substring(0, 20)}...";
                        break;

                    case "passwordreset":
                        var resetUrl = $"https://localhost:7071/Account/ResetPassword?token={token}";
                        result = await _emailService.SendPasswordResetEmailAsync(email, name, resetUrl);
                        EmailOutput = $"Password reset email sent to: {email}\nName: {name}\nReset URL: {resetUrl}";
                        break;

                    case "duereminder":
                        var dueDate = DateTime.Now.AddDays(3);
                        result = await _emailService.SendBookDueReminderAsync(email, name, "The Great Gatsby", dueDate);
                        EmailOutput = $"Due reminder sent to: {email}\nName: {name}\nBook: The Great Gatsby\nDue: {dueDate:MMM dd, yyyy}";
                        break;

                    default:
                        ErrorMessage = "Invalid email type selected.";
                        return Page();
                }

                if (result)
                {
                    SuccessMessage = $"✓ Email sent successfully! Check your console/terminal for DEV MODE output.";
                }
                else
                {
                    ErrorMessage = "Failed to send email. Check console for errors.";
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}
