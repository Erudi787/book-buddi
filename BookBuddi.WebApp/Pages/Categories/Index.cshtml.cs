using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public IndexModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public string? SearchTerm { get; set; }

        public IActionResult OnGet(string? searchTerm)
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Index");
            }

            SearchTerm = searchTerm;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Categories = _categoryService.SearchCategories(searchTerm);
            }
            else
            {
                Categories = _categoryService.GetAllCategories();
            }

            return Page();
        }
    }
}
