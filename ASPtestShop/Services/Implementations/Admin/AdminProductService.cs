using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Product;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ASPtestShop.Services.Implementations.Admin
{
    // Service xử lý CRUD Product cho Admin
    public class AdminProductService : IAdminProductService
    {
        private readonly AppDbContext _context;

        public AdminProductService(AppDbContext context)
        {
            _context = context;
        }

        //=====================================GET=========================================
        // GET: /api/admin/products
        public async Task<List<AdminProductResultDto>> GetProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new AdminProductResultDto
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    ProductName = p.ProductName,
                    Slug = p.Slug,
                    ShortDescription = p.ShortDescription,
                    Description = p.Description,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    StockQuantity = p.StockQuantity,
                    ThumbnailUrl = p.ThumbnailUrl,
                    IsFeatured = p.IsFeatured,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    // Lấy danh sách hình ảnh sản phẩm theo SortOrder
                    Images = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => new ProductImageDto
                        {
                            ProductImageId = img.ProductImageId,
                            ImageUrl = img.ImageUrl,
                            IsPrimary = img.IsPrimary,
                            SortOrder = img.SortOrder
                        })
                        .ToList()
                })

                .ToListAsync();

            return products;
        }

        //====================================GET BY ID===============================
        // GET: /api/admin/products/{productId}
        public async Task<AdminProductResultDto?> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductId == productId)
                .Select(p => new AdminProductResultDto
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    ProductName = p.ProductName,
                    Slug = p.Slug,
                    ShortDescription = p.ShortDescription,
                    Description = p.Description,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    StockQuantity = p.StockQuantity,
                    ThumbnailUrl = p.ThumbnailUrl,
                    IsFeatured = p.IsFeatured,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Images = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => new ProductImageDto
                        {
                            ProductImageId = img.ProductImageId,
                            ImageUrl = img.ImageUrl,
                            IsPrimary = img.IsPrimary,
                            SortOrder = img.SortOrder
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return product;
        }

        //========================================CREATE========================================
        // POST: /api/admin/products
        public async Task<AdminProductActionResultDto> CreateProductAsync(CreateProductDto dto)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == dto.CategoryId && c.IsActive);

            if (!categoryExists)
            {
                return new AdminProductActionResultDto
                {
                    Success = false,
                    Message = "Danh mục không tồn tại hoặc đã bị ẩn"
                };
            }

            var slug = string.IsNullOrWhiteSpace(dto.Slug)
                ? GenerateSlug(dto.ProductName)
                : GenerateSlug(dto.Slug);

            var slugExists = await _context.Products
                .AnyAsync(p => p.Slug == slug);

            if (slugExists)
            {
                return new AdminProductActionResultDto
                {
                    Success = false,
                    Message = "Slug sản phẩm đã tồn tại"
                };
            }

            if (dto.SalePrice.HasValue && dto.SalePrice.Value > dto.Price)
            {
                return new AdminProductActionResultDto
                {
                    Success = false,
                    Message = "Giá khuyến mãi không được lớn hơn giá gốc"
                };
            }

            var product = new Product
            {
                CategoryId = dto.CategoryId,
                ProductName = dto.ProductName,
                Slug = slug,
                ShortDescription = dto.ShortDescription,
                Description = dto.Description,
                Price = dto.Price,
                SalePrice = dto.SalePrice,
                StockQuantity = dto.StockQuantity,
                ThumbnailUrl = dto.ThumbnailUrl,
                IsFeatured = dto.IsFeatured,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            // Lưu hình ảnh sản phẩm nếu có
            if (dto.Images.Any())
            {
                var productImages = dto.Images.Select((img, index) => new ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = img.ImageUrl,
                    IsPrimary = img.IsPrimary,
                    SortOrder = img.SortOrder == 0 ? index + 1 : img.SortOrder
                }).ToList();

                // Nếu admin chưa chọn ảnh chính thì tự lấy ảnh đầu tiên làm ảnh chính
                if (!productImages.Any(img => img.IsPrimary))
                {
                    productImages[0].IsPrimary = true;
                }

                _context.ProductImages.AddRange(productImages);

                // Nếu ThumbnailUrl chưa nhập thì lấy ảnh chính làm thumbnail
                if (string.IsNullOrWhiteSpace(product.ThumbnailUrl))
                {
                    product.ThumbnailUrl = productImages
                        .OrderByDescending(img => img.IsPrimary)
                        .ThenBy(img => img.SortOrder)
                        .First()
                        .ImageUrl;
                }

                await _context.SaveChangesAsync();
            }

            var resultProduct = await GetProductByIdAsync(product.ProductId);

            return new AdminProductActionResultDto
            {
                Success = true,
                Message = "Thêm sản phẩm thành công",
                Product = resultProduct
            };
        }

        //==================================UPDATE===============================
        // PUT: /api/admin/products/{productId}
        public async Task<AdminProductActionResultDto> UpdateProductAsync(int productId, UpdateProductDto dto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return new AdminProductActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                };
            }

            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.CategoryId == dto.CategoryId.Value && c.IsActive);

                if (!categoryExists)
                {
                    return new AdminProductActionResultDto
                    {
                        Success = false,
                        Message = "Danh mục không tồn tại hoặc đã bị ẩn"
                    };
                }

                product.CategoryId = dto.CategoryId.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.ProductName))
            {
                product.ProductName = dto.ProductName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Slug))
            {
                var newSlug = GenerateSlug(dto.Slug);

                var slugExists = await _context.Products
                    .AnyAsync(p => p.Slug == newSlug && p.ProductId != productId);

                if (slugExists)
                {
                    return new AdminProductActionResultDto
                    {
                        Success = false,
                        Message = "Slug sản phẩm đã tồn tại"
                    };
                }

                product.Slug = newSlug;
            }

            if (dto.ShortDescription != null)
            {
                product.ShortDescription = dto.ShortDescription;
            }

            if (dto.Description != null)
            {
                product.Description = dto.Description;
            }

            if (dto.Price.HasValue)
            {
                if (dto.Price.Value < 0)
                {
                    return new AdminProductActionResultDto
                    {
                        Success = false,
                        Message = "Giá sản phẩm không hợp lệ"
                    };
                }

                product.Price = dto.Price.Value;
            }

            if (dto.SalePrice.HasValue)
            {
                if (dto.SalePrice.Value < 0)
                {
                    return new AdminProductActionResultDto
                    {
                        Success = false,
                        Message = "Giá khuyến mãi không hợp lệ"
                    };
                }

                product.SalePrice = dto.SalePrice.Value;
            }

            if (product.SalePrice.HasValue && product.SalePrice.Value > product.Price)
            {
                return new AdminProductActionResultDto
                {
                    Success = false,
                    Message = "Giá khuyến mãi không được lớn hơn giá gốc"
                };
            }

            if (dto.StockQuantity.HasValue)
            {
                if (dto.StockQuantity.Value < 0)
                {
                    return new AdminProductActionResultDto
                    {
                        Success = false,
                        Message = "Số lượng tồn kho không hợp lệ"
                    };
                }

                product.StockQuantity = dto.StockQuantity.Value;
            }

            if (dto.ThumbnailUrl != null)
            {
                product.ThumbnailUrl = dto.ThumbnailUrl;
            }

            if (dto.IsFeatured.HasValue)
            {
                product.IsFeatured = dto.IsFeatured.Value;
            }

            if (dto.IsActive.HasValue)
            {
                product.IsActive = dto.IsActive.Value;
            }
            // Cập nhật hình ảnh sản phẩm nếu có
            if (dto.Images != null)
            {
                var oldImages = await _context.ProductImages
                    .Where(img => img.ProductId == product.ProductId)
                    .ToListAsync();

                _context.ProductImages.RemoveRange(oldImages);

                if (dto.Images.Any())
                {
                    var newImages = dto.Images.Select((img, index) => new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = img.ImageUrl,
                        IsPrimary = img.IsPrimary,
                        SortOrder = img.SortOrder == 0 ? index + 1 : img.SortOrder
                    }).ToList();

                    // Nếu không có ảnh nào được chọn làm ảnh chính thì lấy ảnh đầu tiên
                    if (!newImages.Any(img => img.IsPrimary))
                    {
                        newImages[0].IsPrimary = true;
                    }

                    _context.ProductImages.AddRange(newImages);

                    // Nếu admin không truyền ThumbnailUrl thì lấy ảnh chính làm thumbnail
                    if (string.IsNullOrWhiteSpace(dto.ThumbnailUrl))
                    {
                        product.ThumbnailUrl = newImages
                            .OrderByDescending(img => img.IsPrimary)
                            .ThenBy(img => img.SortOrder)
                            .First()
                            .ImageUrl;
                    }
                }
            }

            product.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var resultProduct = await GetProductByIdAsync(product.ProductId);

            return new AdminProductActionResultDto
            {
                Success = true,
                Message = "Cập nhật sản phẩm thành công",
                Product = resultProduct
            };
        }

        //==================================DELETE===============================
        // DELETE: /api/admin/products/{productId}
        public async Task<AdminProductActionResultDto> DeleteProductAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return new AdminProductActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                };
            }

            // Không xóa thật khỏi database
            // Chỉ ẩn sản phẩm để tránh ảnh hưởng đơn hàng cũ
            product.IsActive = false;
            product.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var resultProduct = await GetProductByIdAsync(product.ProductId);

            return new AdminProductActionResultDto
            {
                Success = true,
                Message = "Ẩn sản phẩm thành công",
                Product = resultProduct
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