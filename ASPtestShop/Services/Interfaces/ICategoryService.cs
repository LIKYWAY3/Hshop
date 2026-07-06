using ASPtestShop.Models.DTO.Category;

namespace ASPtestShop.Services.Interfaces;

// Interface khai báo các chức năng liên quan đến Category
// Controller sẽ gọi interface này, không gọi AppDbContext trực tiếp
public interface ICategoryService
{
    // Lấy tất cả danh mục đang hoạt động
    Task<List<CategoryResultDto>> GetCategoriesAsync();

    // Lấy chi tiết danh mục theo categoryId
    Task<CategoryResultDto?> GetCategoryByIdAsync(int categoryId);

    // Lấy các danh mục cha
    Task<List<CategoryResultDto>> GetParentCategoriesAsync();

    // Lấy danh mục con theo parentCategoryId
    Task<List<CategoryResultDto>> GetSubCategoriesAsync(int parentCategoryId);
}