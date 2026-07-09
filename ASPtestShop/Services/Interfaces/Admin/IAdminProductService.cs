using ASPtestShop.Models.DTO.Product;

namespace ASPtestShop.Services.Interfaces.Admin;

public interface IAdminProductService
{
    // Admin lấy tất cả sản phẩm, bao gồm cả IsActive = false
    Task<List<AdminProductResultDto>> GetProductsAsync();

    // Admin lấy chi tiết sản phẩm theo ProductId
    Task<AdminProductResultDto?> GetProductByIdAsync(int productId);

    // Admin thêm sản phẩm
    Task<AdminProductActionResultDto> CreateProductAsync(CreateProductDto dto);

    // Admin sửa sản phẩm
    Task<AdminProductActionResultDto> UpdateProductAsync(int productId, UpdateProductDto dto);

    // Admin xóa mềm sản phẩm
    Task<AdminProductActionResultDto> DeleteProductAsync(int productId);
}