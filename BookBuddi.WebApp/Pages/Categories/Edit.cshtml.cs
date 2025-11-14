using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public EditModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public CategoryViewModel Category { get; set; } = new CategoryViewModel();
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Admin/Login");
            }

            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return RedirectToPage("./Index");
            }

            Category = category;
            return Page();
        }

        public IActionResult OnPost()
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin)
            {
                return RedirectToPage("/Admin/Login");
            }

            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _categoryService.UpdateCategory(Category, adminName);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
