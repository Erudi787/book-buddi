using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using BookBuddi.Services;
using BookBuddi.Models;
using AdminModel = BookBuddi.Models.Admin;

namespace BookBuddi.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly MemberService _memberService;
        private readonly AuthenticationService _authService;
        private readonly SignInManager<AdminModel> _signInManager;
        private readonly UserManager<AdminModel> _userManager;

        public LoginModel(
            MemberService memberService,
            AuthenticationService authService,
            SignInManager<AdminModel> signInManager,
            UserManager<AdminModel> userManager)
        {
            _memberService = memberService;
            _authService = authService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLoginAsync(string email, string password)
        {
            // First try member login
            var member = await _memberService.GetMemberByEmailAsync(email);

            if (member != null)
            {
                // Verify password
                bool isPasswordValid = _authService.VerifyPassword(password, member.PasswordHash);
                if (!isPasswordValid)
                {
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }

                // Check member status
                if (member.Status != Models.Enums.MemberStatus.Active)
                {
                    ErrorMessage = $"Your account is {member.Status}. Please contact support.";
                    return Page();
                }

                // Store member info in session
                HttpContext.Session.SetInt32("MemberId", member.MemberId);
                HttpContext.Session.SetString("MemberName", $"{member.FirstName} {member.LastName}");
                HttpContext.Session.SetString("UserRole", "Member");

                return RedirectToPage("/Index");
            }

            // Try admin login
            var admin = await _userManager.FindByEmailAsync(email);
            if (admin != null)
            {
                var result = await _signInManager.PasswordSignInAsync(admin.UserName!, password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("UserRole", "Admin");
                    HttpContext.Session.SetString("AdminName", $"{admin.FirstName} {admin.LastName}");
                    return RedirectToPage("/Index");
                }
            }

            ErrorMessage = "Invalid email or password.";
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterAsync(
            string firstName, string lastName, string email,
            string password, string confirmPassword,
            string? phone, string? address, DateTime? dateOfBirth)
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

            // Check if email already exists
            var existingMember = await _memberService.GetMemberByEmailAsync(email);
            if (existingMember != null)
            {
                ErrorMessage = "An account with this email already exists.";
                return Page();
            }

            // Hash password
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
                Status = Models.Enums.MemberStatus.Active,
                BorrowingLimit = 5,
                MembershipExpiryDate = DateTime.Now.AddYears(1)
            };

            await _memberService.RegisterMemberAsync(member);
            SuccessMessage = "Registration successful! You can now login.";
            return Page();
        }
    }
}
