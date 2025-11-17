using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Services.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace BookBuddi.Pages.Members
{
    public class CreateModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly IEmailService _emailService;
        private readonly ApplicationSettings _appSettings;

        public CreateModel(
            IMemberService memberService,
            IEmailService emailService,
            IOptions<ApplicationSettings> appSettings)
        {
            _memberService = memberService;
            _emailService = emailService;
            _appSettings = appSettings.Value;
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

        public async Task<IActionResult> OnPostAsync(string password)
        {
            Console.WriteLine("=== OnPostAsync called in Members/Create ===");

            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid");
                return Page();
            }

            Console.WriteLine($"Creating member account for: {Member.Email}");

            try
            {
                // Generate email verification token and code
                var verificationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                var verificationCode = new Random().Next(100000, 999999).ToString(); // 6-digit code
                var tokenExpiry = DateTime.Now.AddHours(_appSettings.TokenExpirationHours);

                Console.WriteLine($"Generated verification code: {verificationCode}");

                // Set verification fields
                Member.EmailVerified = false;
                Member.EmailVerificationToken = verificationToken;
                Member.EmailVerificationCode = verificationCode;
                Member.EmailVerificationTokenExpiry = tokenExpiry;

                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _memberService.AddMember(Member, password, adminName);

                // Send verification email
                try
                {
                    Console.WriteLine($"Attempting to send verification email to {Member.Email}...");
                    var emailResult = await _emailService.SendMemberVerificationEmailAsync(
                        Member.Email,
                        Member.FirstName,
                        verificationToken,
                        verificationCode);

                    if (emailResult)
                    {
                        Console.WriteLine($"Verification email sent successfully to {Member.Email}");
                        TempData["SuccessMessage"] = $"Member account created successfully! A verification email has been sent to {Member.Email}.";
                    }
                    else
                    {
                        Console.WriteLine($"Email service returned false for {Member.Email}");
                        TempData["WarningMessage"] = $"Member account created, but email sending returned false. Check logs for details.";
                    }
                }
                catch (Exception emailEx)
                {
                    // Log email error but don't fail member creation
                    Console.WriteLine($"Failed to send verification email: {emailEx.Message}");
                    Console.WriteLine($"Stack trace: {emailEx.StackTrace}");
                    TempData["WarningMessage"] = $"Member account created, but failed to send verification email: {emailEx.Message}";
                }

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
