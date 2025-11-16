using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Data.Models;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.Manager;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Pages.Admin
{
    public class CreateAccountModel : PageModel
    {
        private readonly IMemberService _memberService;
        private readonly UserManager<Data.Models.Admin> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PasswordManager _passwordManager;

        public CreateAccountModel(
            IMemberService memberService,
            UserManager<Data.Models.Admin> userManager,
            RoleManager<IdentityRole> roleManager,
            PasswordManager passwordManager)
        {
            _memberService = memberService;
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordManager = passwordManager;
        }

        [BindProperty]
        public string FirstName { get; set; } = string.Empty;

        [BindProperty]
        public string LastName { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        public string AccountType { get; set; } = "Member"; // Member or Admin

        // Member-specific fields
        [BindProperty]
        public string? Phone { get; set; }

        [BindProperty]
        public string? Address { get; set; }

        [BindProperty]
        public DateTime? DateOfBirth { get; set; }

        [BindProperty]
        public int BorrowingLimit { get; set; } = 5;

        [BindProperty]
        public DateTime MembershipExpiryDate { get; set; } = DateTime.Now.AddYears(1);

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

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

        public async Task<IActionResult> OnPostAsync()
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");
            }

            try
            {
                // Validate passwords match
                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    return Page();
                }

                // Validate password strength
                var (isValid, validationMessage) = _passwordManager.ValidatePassword(Password);
                if (!isValid)
                {
                    ErrorMessage = validationMessage;
                    return Page();
                }

                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";

                if (AccountType == "Admin")
                {
                    // Create Admin account
                    var existingAdmin = await _userManager.FindByEmailAsync(Email);
                    if (existingAdmin != null)
                    {
                        ErrorMessage = "An admin account with this email already exists.";
                        return Page();
                    }

                    // Ensure Admin role exists
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }

                    var newAdmin = new Data.Models.Admin
                    {
                        UserName = Email,
                        Email = Email,
                        FirstName = FirstName,
                        LastName = LastName,
                        EmailConfirmed = true,
                        IsActive = true,
                        DateCreated = DateTime.Now,
                        CreatedBy = adminName,
                        CreatedTime = DateTime.Now,
                        UpdatedBy = adminName,
                        UpdatedTime = DateTime.Now
                    };

                    var result = await _userManager.CreateAsync(newAdmin, Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newAdmin, "Admin");
                        SuccessMessage = $"Admin account created successfully for {FirstName} {LastName}.";

                        // Clear form
                        ModelState.Clear();
                        FirstName = string.Empty;
                        LastName = string.Empty;
                        Email = string.Empty;

                        return Page();
                    }
                    else
                    {
                        ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                        return Page();
                    }
                }
                else
                {
                    // Create Member account
                    if (_memberService.EmailExists(Email))
                    {
                        ErrorMessage = "A member with this email already exists.";
                        return Page();
                    }

                    var memberViewModel = new MemberViewModel
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Phone = Phone,
                        Address = Address,
                        DateOfBirth = DateOfBirth,
                        Status = MemberStatus.Active,
                        BorrowingLimit = BorrowingLimit,
                        MembershipDate = DateTime.Now,
                        MembershipExpiryDate = MembershipExpiryDate
                    };

                    _memberService.AddMember(memberViewModel, Password, adminName);
                    SuccessMessage = $"Member account created successfully for {FirstName} {LastName}.";

                    // Clear form
                    ModelState.Clear();
                    FirstName = string.Empty;
                    LastName = string.Empty;
                    Email = string.Empty;
                    Phone = null;
                    Address = null;
                    DateOfBirth = null;
                    BorrowingLimit = 5;
                    MembershipExpiryDate = DateTime.Now.AddYears(1);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating account: {ex.Message}";
                return Page();
            }
        }
    }
}
