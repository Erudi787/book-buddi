using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public DeleteModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

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

        public IActionResult OnPost(int id)
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin)
            {
                return RedirectToPage("/Admin/Login");
            }

            try
            {
                _categoryService.DeleteCategory(id);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                var category = _categoryService.GetCategoryById(id);
                if (category != null)
                {
                    Category = category;
                }
                return Page();
            }
        }
    }
}
