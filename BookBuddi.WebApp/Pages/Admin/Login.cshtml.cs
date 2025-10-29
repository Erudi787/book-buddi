using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Data.Models;

namespace BookBuddi.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Data.Models.Admin> _signInManager;

        public LoginModel(SignInManager<Data.Models.Admin> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Store admin info in session
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("AdminEmail", Input.Email);
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid login attempt. Please check your credentials.";
            return Page();
        }
    }
}
