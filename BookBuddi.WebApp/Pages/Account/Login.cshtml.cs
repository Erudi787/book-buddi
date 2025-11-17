using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.Manager;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Services.Configuration;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using Microsoft.Extensions.Options;
using AdminModel = BookBuddi.Data.Models.Admin;

namespace BookBuddi.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly PasswordManager _passwordManager;
        private readonly SignInManager<AdminModel> _signInManager;
        private readonly UserManager<AdminModel> _userManager;
        private readonly ApplicationSettings _appSettings;

        public LoginModel(
            IMemberService memberService,
            PasswordManager passwordManager,
            SignInManager<AdminModel> signInManager,
            UserManager<AdminModel> userManager,
            IOptions<ApplicationSettings> appSettings)
        {
            _memberService = memberService;
            _passwordManager = passwordManager;
            _signInManager = signInManager;
            _userManager = userManager;
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

        public async Task<IActionResult> OnPostLoginAsync(string email, string password)
        {
            // First try member login using credentials validation
            bool isValidMember = _memberService.ValidateCredentials(email, password);

            if (isValidMember)
            {
                var member = _memberService.GetMemberByEmail(email);

                if (member == null)
                {
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }

                // Check email verification if required
                if (_appSettings.EmailVerificationRequired && !member.EmailVerified)
                {
                    ErrorMessage = "Please verify your email address before logging in. Check your inbox for the verification link.";
                    return Page();
                }

                // Check member status
                if (member.Status != MemberStatus.Active)
                {
                    ErrorMessage = $"Your account is {member.Status}. Please contact support.";
                    return Page();
                }

                // Store member info in session
                HttpContext.Session.SetInt32("MemberId", member.MemberId);
                HttpContext.Session.SetString("MemberName", $"{member.FirstName} {member.LastName}");
                HttpContext.Session.SetString("UserRole", "Member");

                return RedirectToPage("/Books/Index");
            }

            // Try admin login
            var admin = await _userManager.FindByEmailAsync(email);
            if (admin != null)
            {
                var result = await _signInManager.PasswordSignInAsync(admin.UserName!, password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Check if admin account is active
                    if (!admin.IsActive)
                    {
                        ErrorMessage = "Your admin account has been deactivated. Please contact support.";
                        await _signInManager.SignOutAsync();
                        return Page();
                    }

                    HttpContext.Session.SetString("UserRole", "Admin");
                    HttpContext.Session.SetString("AdminName", $"{admin.FirstName} {admin.LastName}");
                    HttpContext.Session.SetString("AdminEmail", admin.Email!);
                    return RedirectToPage("/Admin/Index");
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
            var (isValid, validationMessage) = _passwordManager.ValidatePassword(password);
            if (!isValid)
            {
                ErrorMessage = validationMessage;
                return Page();
            }

            // Check if email already exists
            var existingMember = _memberService.GetMemberByEmail(email);
            if (existingMember != null)
            {
                ErrorMessage = "An account with this email already exists.";
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
                MembershipExpiryDate = DateTime.Now.AddYears(1)
            };

            _memberService.AddMember(memberViewModel, password, "System");
            SuccessMessage = "Registration successful! You can now login.";
            return Page();
        }
    }
}
