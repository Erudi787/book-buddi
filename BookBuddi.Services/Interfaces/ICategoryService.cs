using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<CategoryViewModel> GetAllCategories();
        CategoryViewModel? GetCategoryById(int categoryId);
        IEnumerable<CategoryViewModel> SearchCategories(string searchTerm);
        bool CategoryExists(int categoryId);
        bool CategoryNameExists(string categoryName, int? excludeCategoryId = null);
        void AddCategory(CategoryViewModel model, string createdBy);
        void UpdateCategory(CategoryViewModel model, string updatedBy);
        void DeleteCategory(int categoryId);
    }
}
