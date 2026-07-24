using ASPtestShop.Models.DTO.Product;

namespace ASPtestShop.Services.Interfaces;

public interface IProductService
{
    // 1. Lấy danh sách tất cả sản phẩm
    // Dùng cho: GET /api/products
    Task<List<ProductListItemDto>> GetProductsAsync();

    // 2. Lấy danh sách sản phẩm theo categoryId
    // Dùng cho: GET /api/products/category/{categoryId}
    Task<List<ProductListItemDto>> GetProductsByCategoryAsync(int categoryId);

    // 3. Lấy chi tiết sản phẩm theo productId
    // Dùng cho: GET /api/products/{productId}
    Task<ProductDetailDto?> GetProductByIdAsync(int productId);

    // 4. Lấy danh sách sản phẩm nổi bật
    // Dùng cho: GET /api/products/featured
    Task<List<ProductListItemDto>> GetFeaturedProductsAsync();

    // 5. Tìm kiếm sản phẩm theo keyword (toàn bộ shop)
    // Dùng cho: GET /api/products/search?keyword=abc
    Task<List<ProductListItemDto>> SearchProductsAsync(string keyword);

    // 6. Tìm kiếm sản phẩm theo keyword trong danh mục cụ thể (bao gồm danh mục con)
    // Dùng cho: GET /api/products/search?keyword=abc&categoryId=1
    Task<List<ProductListItemDto>> SearchProductsInCategoryAsync(string keyword, int categoryId);
}