using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Category> GetCategories()
        {
            return this.GetDbSet<Category>().OrderBy(c => c.CategoryName);
        }

        public Category? GetCategoryById(int categoryId)
        {
            return this.GetDbSet<Category>().FirstOrDefault(c => c.CategoryId == categoryId);
        }

        public IEnumerable<Category> SearchCategories(string searchTerm)
        {
            return this.GetDbSet<Category>()
                .Where(c => c.CategoryName.Contains(searchTerm) ||
                           (c.Description != null && c.Description.Contains(searchTerm)))
                .OrderBy(c => c.CategoryName)
                .ToList();
        }

        public bool CategoryExists(int categoryId)
        {
            return this.GetDbSet<Category>().Any(c => c.CategoryId == categoryId);
        }

        public bool CategoryNameExists(string categoryName, int? excludeCategoryId = null)
        {
            var query = this.GetDbSet<Category>()
                .Where(c => c.CategoryName.ToLower() == categoryName.ToLower());

            if (excludeCategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId != excludeCategoryId.Value);
            }

            return query.Any();
        }

        public void AddCategory(Category category)
        {
            this.GetDbSet<Category>().Add(category);
            UnitOfWork.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            SetEntityState(category, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteCategory(Category category)
        {
            this.GetDbSet<Category>().Remove(category);
            UnitOfWork.SaveChanges();
        }
    }
}
