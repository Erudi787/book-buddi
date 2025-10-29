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
                    MembershipExpiryDate = DateTime.Now.AddYears(1) // 1 year membership
                };

                _memberService.AddMember(memberViewModel, password, "System");
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
