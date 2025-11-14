using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public IEnumerable<CategoryViewModel> GetAllCategories()
        {
            var categories = _categoryRepository.GetCategories().ToList();
            return _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
        }

        public CategoryViewModel? GetCategoryById(int categoryId)
        {
            var category = _categoryRepository.GetCategoryById(categoryId);
            return category != null ? _mapper.Map<CategoryViewModel>(category) : null;
        }

        public IEnumerable<CategoryViewModel> SearchCategories(string searchTerm)
        {
            var categories = _categoryRepository.SearchCategories(searchTerm);
            return _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
        }

        public bool CategoryExists(int categoryId)
        {
            return _categoryRepository.CategoryExists(categoryId);
        }

        public bool CategoryNameExists(string categoryName, int? excludeCategoryId = null)
        {
            return _categoryRepository.CategoryNameExists(categoryName, excludeCategoryId);
        }

        public void AddCategory(CategoryViewModel model, string createdBy)
        {
            // Check if category name already exists
            if (_categoryRepository.CategoryNameExists(model.CategoryName))
            {
                throw new InvalidOperationException($"A category with the name '{model.CategoryName}' already exists.");
            }

            var category = _mapper.Map<Category>(model);
            category.CreatedBy = createdBy;
            category.CreatedTime = DateTime.Now;
            category.UpdatedBy = createdBy;
            category.UpdatedTime = DateTime.Now;

            _categoryRepository.AddCategory(category);
        }

        public void UpdateCategory(CategoryViewModel model, string updatedBy)
        {
            var category = _categoryRepository.GetCategoryById(model.CategoryId);
            if (category == null)
                throw new InvalidOperationException("Category not found");

            // Check if new name conflicts with existing category
            if (_categoryRepository.CategoryNameExists(model.CategoryName, model.CategoryId))
            {
                throw new InvalidOperationException($"A category with the name '{model.CategoryName}' already exists.");
            }

            _mapper.Map(model, category);
            category.UpdatedBy = updatedBy;
            category.UpdatedTime = DateTime.Now;

            _categoryRepository.UpdateCategory(category);
        }

        public void DeleteCategory(int categoryId)
        {
            var category = _categoryRepository.GetCategoryById(categoryId);
            if (category == null)
                throw new InvalidOperationException("Category not found");

            _categoryRepository.DeleteCategory(category);
        }
    }
}
