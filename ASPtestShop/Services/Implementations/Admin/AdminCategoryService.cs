using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Category;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly AppDbContext _context;

        public AdminCategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminCategoryResultDto>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new AdminCategoryResultDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null
                        ? c.ParentCategory.CategoryName
                        : null,
                    ProductCount = c.Products.Count,
                    SubCategoryCount = c.SubCategories.Count,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return categories;
        }

        public async Task<AdminCategoryResultDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new AdminCategoryResultDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null
                        ? c.ParentCategory.CategoryName
                        : null,
                    ProductCount = c.Products.Count,
                    SubCategoryCount = c.SubCategories.Count,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return category;
        }

        public async Task<AdminCategoryActionResultDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var slug = string.IsNullOrWhiteSpace(dto.Slug)
                ? GenerateSlug(dto.CategoryName)
                : GenerateSlug(dto.Slug);

            var slugExists = await _context.Categories
                .AnyAsync(c => c.Slug == slug);

            if (slugExists)
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Slug danh mục đã tồn tại"
                };
            }

            if (dto.ParentCategoryId.HasValue)
            {
                var parentExists = await _context.Categories
                    .AnyAsync(c =>
                        c.CategoryId == dto.ParentCategoryId.Value &&
                        c.IsActive);

                if (!parentExists)
                {
                    return new AdminCategoryActionResultDto
                    {
                        Success = false,
                        Message = "Danh mục cha không tồn tại hoặc đã bị ẩn"
                    };
                }
            }

            var category = new Category
            {
                CategoryName = dto.CategoryName,
                Slug = slug,
                Description = dto.Description,
                ParentCategoryId = dto.ParentCategoryId,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var resultCategory = await GetCategoryByIdAsync(category.CategoryId);

            return new AdminCategoryActionResultDto
            {
                Success = true,
                Message = "Thêm danh mục thành công",
                Category = resultCategory
            };
        }

        public async Task<AdminCategoryActionResultDto> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục"
                };
            }

            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                category.CategoryName = dto.CategoryName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Slug))
            {
                var newSlug = GenerateSlug(dto.Slug);

                var slugExists = await _context.Categories
                    .AnyAsync(c => c.Slug == newSlug && c.CategoryId != categoryId);

                if (slugExists)
                {
                    return new AdminCategoryActionResultDto
                    {
                        Success = false,
                        Message = "Slug danh mục đã tồn tại"
                    };
                }

                category.Slug = newSlug;
            }

            if (dto.Description != null)
            {
                category.Description = dto.Description;
            }

            if (dto.ParentCategoryId.HasValue)
            {
                if (dto.ParentCategoryId.Value == categoryId)
                {
                    return new AdminCategoryActionResultDto
                    {
                        Success = false,
                        Message = "Danh mục không thể chọn chính nó làm danh mục cha"
                    };
                }

                var parentExists = await _context.Categories
                    .AnyAsync(c =>
                        c.CategoryId == dto.ParentCategoryId.Value &&
                        c.IsActive);

                if (!parentExists)
                {
                    return new AdminCategoryActionResultDto
                    {
                        Success = false,
                        Message = "Danh mục cha không tồn tại hoặc đã bị ẩn"
                    };
                }

                category.ParentCategoryId = dto.ParentCategoryId.Value;
            }

            if (dto.IsActive.HasValue)
            {
                category.IsActive = dto.IsActive.Value;
            }

            category.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var resultCategory = await GetCategoryByIdAsync(category.CategoryId);

            return new AdminCategoryActionResultDto
            {
                Success = true,
                Message = "Cập nhật danh mục thành công",
                Category = resultCategory
            };
        }

        public async Task<AdminCategoryActionResultDto> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục"
                };
            }

            if (category.Products.Any(p => p.IsActive))
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Không thể ẩn danh mục vì vẫn còn sản phẩm đang hoạt động"
                };
            }

            if (category.SubCategories.Any(c => c.IsActive))
            {
                return new AdminCategoryActionResultDto
                {
                    Success = false,
                    Message = "Không thể ẩn danh mục vì vẫn còn danh mục con đang hoạt động"
                };
            }

            category.IsActive = false;
            category.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var resultCategory = await GetCategoryByIdAsync(category.CategoryId);

            return new AdminCategoryActionResultDto
            {
                Success = true,
                Message = "Ẩn danh mục thành công",
                Category = resultCategory
            };
        }

        private static string GenerateSlug(string value)
        {
            value = value.ToLower().Trim();

            value = Regex.Replace(value, @"\s+", "-");

            value = Regex.Replace(value, @"[^a-z0-9\-]", "");

            value = Regex.Replace(value, @"-+", "-");

            return value.Trim('-');
        }
    }
}