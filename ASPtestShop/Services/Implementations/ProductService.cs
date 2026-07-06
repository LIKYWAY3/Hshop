using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Product;
using ASPtestShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    // ProductService chứa toàn bộ logic xử lý sản phẩm
    // Đây là nơi gọi AppDbContext thay cho Controller
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        // Inject AppDbContext để service có thể truy cập database
        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả sản phẩm
        // Code này được tách từ API GET /api/products cũ
        public async Task<List<ProductListItemDto>> GetProductsAsync()
        {
            var products = await _context.Products
                // Select trực tiếp sang DTO, không trả Entity Product ra ngoài
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,

                    // Nếu có SalePrice thì lấy SalePrice
                    // Nếu SalePrice null thì lấy Price
                    Price = p.SalePrice ?? p.Price,

                    // Giá gốc
                    OriginalPrice = p.Price,

                    // Ảnh đại diện trong bảng Products
                    ThumbnailUrl = p.ThumbnailUrl,

                    // Lấy ảnh đầu tiên trong ProductImages theo SortOrder
                    ImageUrl = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return products;
        }

        // Lấy danh sách sản phẩm theo categoryId
        // Code này được tách từ API GET /api/products/category/{categoryId}
        public async Task<List<ProductListItemDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                // Lọc sản phẩm theo CategoryId
                .Where(p => p.CategoryId == categoryId)

                // Map sang DTO
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,
                    ThumbnailUrl = p.ThumbnailUrl,

                    // Lấy ảnh đầu tiên của sản phẩm
                    ImageUrl = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return products;
        }

        // Lấy chi tiết một sản phẩm theo id
        // Code này được tách từ API GET /api/products/{id}
        public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                // Tìm sản phẩm theo ProductId
                .Where(p => p.ProductId == id)

                // Map sang DTO chi tiết
                .Select(p => new ProductDetailDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,

                    // Giá bán thực tế
                    Price = p.SalePrice ?? p.Price,

                    // Giá gốc
                    OriginalPrice = p.Price,

                    // Số lượng tồn kho
                    StockQuantity = p.StockQuantity,

                    // Ảnh đại diện
                    ThumbnailUrl = p.ThumbnailUrl,

                    // Lấy toàn bộ ảnh phụ của sản phẩm
                    Images = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return product;
        }

        // Lấy danh sách sản phẩm nổi bật
        // Code này được tách từ API GET /api/products/featured
        public async Task<List<ProductListItemDto>> GetFeaturedProductsAsync()
        {
            var products = await _context.Products
                // Chỉ lấy sản phẩm nổi bật
                .Where(p => p.IsFeatured)

                // Map sang DTO
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,
                    ThumbnailUrl = p.ThumbnailUrl,

                    // Lấy ảnh đầu tiên
                    ImageUrl = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return products;
        }

        // Tìm kiếm sản phẩm theo keyword
        // Code này được tách từ API GET /api/products/search?keyword=abc
        public async Task<List<ProductListItemDto>> SearchProductsAsync(string keyword)
        {
            var products = await _context.Products
                // Tìm sản phẩm có ProductName chứa keyword
                .Where(p => p.ProductName.Contains(keyword))

                // Map sang DTO
                .Select(p => new ProductListItemDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,
                    ThumbnailUrl = p.ThumbnailUrl,

                    // Lấy ảnh đầu tiên
                    ImageUrl = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return products;
        }
    }
}