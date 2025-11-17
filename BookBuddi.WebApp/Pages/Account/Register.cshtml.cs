using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.Manager;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Services.Configuration;
using BookBuddi.Resources.Constants;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace BookBuddi.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly PasswordManager _passwordManager;
        private readonly IEmailService _emailService;
        private readonly ApplicationSettings _appSettings;

        public RegisterModel(
            IMemberService memberService,
            PasswordManager passwordManager,
            IEmailService emailService,
            IOptions<ApplicationSettings> appSettings)
        {
            _memberService = memberService;
            _passwordManager = passwordManager;
            _emailService = emailService;
            _appSettings = appSettings.Value;
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

        public async Task<IActionResult> OnPostAsync(string firstName, string lastName, string email,
            string password, string confirmPassword, string? phone, string? address, DateTime? dateOfBirth)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(firstName))
                {
                    ErrorMessage = "First name is required.";
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(lastName))
                {
                    ErrorMessage = "Last name is required.";
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    ErrorMessage = "Email is required.";
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    ErrorMessage = "Password is required.";
                    return Page();
                }

                // Validate passwords match
                if (password != confirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    return Page();
                }

                // Check if email already exists
                if (_memberService.EmailExists(email))
                {
                    ErrorMessage = "An account with this email already exists. Please use a different email or try logging in.";
                    return Page();
                }

                // Validate password strength
                var (isValid, validationMessage) = _passwordManager.ValidatePassword(password);
                if (!isValid)
                {
                    ErrorMessage = validationMessage;
                    return Page();
                }

                // Generate email verification token and code
                var verificationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                var verificationCode = new Random().Next(100000, 999999).ToString(); // 6-digit code
                var tokenExpiry = DateTime.Now.AddHours(_appSettings.TokenExpirationHours);

                var memberViewModel = new MemberViewModel
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = phone,
                    Address = address,
                    DateOfBirth = dateOfBirth,
                    Status = MemberStatus.Active,
                    BorrowingLimit = 5,
                    MembershipDate = DateTime.Now,
                    MembershipExpiryDate = DateTime.Now.AddYears(1), // 1 year membership
                    CurrentBorrowedCount = 0,
                    EmailVerified = false,
                    EmailVerificationToken = verificationToken,
                    EmailVerificationCode = verificationCode,
                    EmailVerificationTokenExpiry = tokenExpiry
                };

                _memberService.AddMember(memberViewModel, password, "Self-Registration");

                // Send verification email with code
                try
                {
                    await _emailService.SendMemberVerificationEmailAsync(email, firstName, verificationToken, verificationCode);
                }
                catch (Exception emailEx)
                {
                    // Log email error but don't fail registration
                    Console.WriteLine($"Failed to send verification email: {emailEx.Message}");
                }

                if (_appSettings.EmailVerificationRequired)
                {
                    // Redirect to verification code entry page
                    return RedirectToPage("/Account/EnterVerificationCode", new { email = email });
                }
                else
                {
                    SuccessMessage = "Registration successful! You can now login with your email and password.";
                    ModelState.Clear();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
                // Log the full exception for debugging
                Console.WriteLine($"Registration error: {ex}");
                return Page();
            }
        }
    }
}
