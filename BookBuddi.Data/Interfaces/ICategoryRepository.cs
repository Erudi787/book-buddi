using BookBuddi.Data.Models;

namespace BookBuddi.Data.Interfaces
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetCategories();
        Category? GetCategoryById(int categoryId);
        IEnumerable<Category> SearchCategories(string searchTerm);
        bool CategoryExists(int categoryId);
        bool CategoryNameExists(string categoryName, int? excludeCategoryId = null);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
    }
}
