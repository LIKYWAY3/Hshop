using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Category;
using ASPtestShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/categories
        public async Task<List<CategoryResultDto>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryId)
                .Select(c => new CategoryResultDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,

                    ParentCategoryName = c.ParentCategory != null
                        ? c.ParentCategory.CategoryName
                        : null,

                    ProductCount = c.Products.Count(p => p.IsActive),

                    SubCategoryCount = c.SubCategories.Count(sc => sc.IsActive),

                    IsActive = c.IsActive
                })
                .ToListAsync();

            return categories;
        }

        // GET /api/categories/{categoryId}
        public async Task<CategoryResultDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryId == categoryId && c.IsActive)
                .Select(c => new CategoryResultDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,

                    ParentCategoryName = c.ParentCategory != null
                        ? c.ParentCategory.CategoryName
                        : null,

                    ProductCount = c.Products.Count(p => p.IsActive),

                    SubCategoryCount = c.SubCategories.Count(sc => sc.IsActive),

                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

            return category;
        }

        // GET /api/categories/parents
        public async Task<List<CategoryResultDto>> GetParentCategoriesAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.ParentCategoryId == null && c.IsActive)
                .OrderBy(c => c.CategoryId)
                .Select(c => new CategoryResultDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = null,

                    ProductCount = c.Products.Count(p => p.IsActive),

                    SubCategoryCount = c.SubCategories.Count(sc => sc.IsActive),

                    IsActive = c.IsActive
                })
                .ToListAsync();

            return categories;
        }

        // GET /api/categories/{parentCategoryId}/subcategories
        public async Task<List<CategoryResultDto>> GetSubCategoriesAsync(int parentCategoryId)
        {
            var categories = await _context.Categories
                .Where(c => c.ParentCategoryId == parentCategoryId && c.IsActive)
                .OrderBy(c => c.CategoryId)
                .Select(c => new CategoryResultDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,

                    ParentCategoryName = c.ParentCategory != null
                        ? c.ParentCategory.CategoryName
                        : null,

                    ProductCount = c.Products.Count(p => p.IsActive),

                    SubCategoryCount = c.SubCategories.Count(sc => sc.IsActive),

                    IsActive = c.IsActive
                })
                .ToListAsync();

            return categories;
        }
    }
}