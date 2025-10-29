using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services;
using BookBuddi.Models;

namespace BookBuddi.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly MemberService _memberService;
        private readonly AuthenticationService _authService;

        public RegisterModel(MemberService memberService, AuthenticationService authService)
        {
            _memberService = memberService;
            _authService = authService;
        }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string firstName, string lastName, string email,
            string password, string confirmPassword, string? phone, string? address, DateTime? dateOfBirth)
        {
            try
            {
                // Validate passwords match
                if (password != confirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    return Page();
                }

                // Validate password strength
                var (isValid, validationMessage) = _authService.ValidatePassword(password);
                if (!isValid)
                {
                    ErrorMessage = validationMessage;
                    return Page();
                }

                // Hash the password
                string passwordHash = _authService.HashPassword(password);

                var member = new Member
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PasswordHash = passwordHash,
                    Phone = phone,
                    Address = address,
                    DateOfBirth = dateOfBirth,
                    MembershipExpiryDate = DateTime.Now.AddYears(1) // 1 year membership
                };

                await _memberService.RegisterMemberAsync(member);
                SuccessMessage = "Registration successful! You can now login.";
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
