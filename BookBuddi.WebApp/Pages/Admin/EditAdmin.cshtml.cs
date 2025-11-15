using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookBuddi.Pages.Admin
{
    public class EditAdminModel : PageModel
    {
        private readonly UserManager<Data.Models.Admin> _userManager;

        public EditAdminModel(UserManager<Data.Models.Admin> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string AdminId { get; set; } = string.Empty;

        [BindProperty]
        public string FirstName { get; set; } = string.Empty;

        [BindProperty]
        public string LastName { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public bool IsActive { get; set; }

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? ConfirmPassword { get; set; }

        public Data.Models.Admin? AdminUser { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Admin ID is required.";
                return RedirectToPage("/Admin/ManageAdmins");
            }

            var admin = await _userManager.FindByIdAsync(id);
            if (admin == null)
            {
                TempData["ErrorMessage"] = "Admin not found.";
                return RedirectToPage("/Admin/ManageAdmins");
            }

            AdminUser = admin;
            AdminId = admin.Id;
            FirstName = admin.FirstName;
            LastName = admin.LastName;
            Email = admin.Email ?? string.Empty;
            IsActive = admin.IsActive;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            if (string.IsNullOrEmpty(AdminId))
            {
                TempData["ErrorMessage"] = "Admin ID is required.";
                return RedirectToPage("/Admin/ManageAdmins");
            }

            try
            {
                var admin = await _userManager.FindByIdAsync(AdminId);
                if (admin == null)
                {
                    ErrorMessage = "Admin not found.";
                    return Page();
                }

                // Store the admin for display
                AdminUser = admin;

                // Basic validation
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "First name, last name, and email are required.";
                    return Page();
                }

                // Check if email is already in use by another admin
                var existingAdmin = await _userManager.FindByEmailAsync(Email);
                if (existingAdmin != null && existingAdmin.Id != AdminId)
                {
                    ErrorMessage = "An admin with this email already exists.";
                    return Page();
                }

                // Update basic info
                admin.FirstName = FirstName;
                admin.LastName = LastName;
                admin.Email = Email;
                admin.UserName = Email;
                admin.IsActive = IsActive;
                admin.UpdatedBy = HttpContext.Session.GetString("AdminName") ?? "Admin";
                admin.UpdatedTime = DateTime.Now;

                var result = await _userManager.UpdateAsync(admin);
                if (!result.Succeeded)
                {
                    ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Page();
                }

                // Handle password change if provided
                if (!string.IsNullOrWhiteSpace(NewPassword))
                {
                    if (NewPassword != ConfirmPassword)
                    {
                        ErrorMessage = "Passwords do not match.";
                        return Page();
                    }

                    // Remove old password and set new one
                    var removePasswordResult = await _userManager.RemovePasswordAsync(admin);
                    if (!removePasswordResult.Succeeded)
                    {
                        ErrorMessage = "Failed to update password.";
                        return Page();
                    }

                    var addPasswordResult = await _userManager.AddPasswordAsync(admin, NewPassword);
                    if (!addPasswordResult.Succeeded)
                    {
                        ErrorMessage = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
                        return Page();
                    }

                    TempData["SuccessMessage"] = $"Admin {FirstName} {LastName} and password updated successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Admin {FirstName} {LastName} updated successfully.";
                }

                return RedirectToPage("/Admin/ManageAdmins");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating admin: {ex.Message}";
                return Page();
            }
        }
    }
}
