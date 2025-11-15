using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Pages.Admin
{
    public class ManageAdminsModel : PageModel
    {
        private readonly UserManager<Data.Models.Admin> _userManager;

        public ManageAdminsModel(UserManager<Data.Models.Admin> userManager)
        {
            _userManager = userManager;
        }

        public List<Data.Models.Admin> Admins { get; set; } = new List<Data.Models.Admin>();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            Admins = await _userManager.Users.OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToListAsync();

            if (TempData["SuccessMessage"] != null)
            {
                SuccessMessage = TempData["SuccessMessage"]?.ToString();
            }

            if (TempData["ErrorMessage"] != null)
            {
                ErrorMessage = TempData["ErrorMessage"]?.ToString();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(string adminId)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            try
            {
                var admin = await _userManager.FindByIdAsync(adminId);
                if (admin == null)
                {
                    TempData["ErrorMessage"] = "Admin not found.";
                    return RedirectToPage();
                }

                // Don't allow deactivating yourself
                var currentAdminEmail = HttpContext.Session.GetString("AdminEmail");
                if (admin.Email == currentAdminEmail)
                {
                    TempData["ErrorMessage"] = "You cannot deactivate your own account.";
                    return RedirectToPage();
                }

                admin.IsActive = !admin.IsActive;
                admin.UpdatedBy = HttpContext.Session.GetString("AdminName") ?? "Admin";
                admin.UpdatedTime = DateTime.Now;

                var result = await _userManager.UpdateAsync(admin);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Admin {admin.FirstName} {admin.LastName} has been {(admin.IsActive ? "activated" : "deactivated")}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update admin status.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}
