using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.Manager;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly PasswordManager _passwordManager;

        public RegisterModel(IMemberService memberService, PasswordManager passwordManager)
        {
            _memberService = memberService;
            _passwordManager = passwordManager;
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
                    CurrentBorrowedCount = 0
                };

                _memberService.AddMember(memberViewModel, password, "Self-Registration");
                SuccessMessage = "Registration successful! You can now login with your email and password.";

                // Clear form data
                ModelState.Clear();

                return Page();
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
