using ASPtestShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Controllers.Api
{
    [Route("api/products")]
    [ApiController]
    public class ProductApiController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/products
        // Lấy danh sách sản phẩm
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,
                    p.ThumbnailUrl,
                    ImageUrl = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }


        // GET: api/products/category/id
        // Lấy danh sách sản phẩm từ categoryId
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,
                    p.ThumbnailUrl,
                    ImageUrl = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/products/1
        // Lấy chi tiết 1 sản phẩm
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Description,
                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,
                    p.StockQuantity,
                    p.ThumbnailUrl,

                    Images = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new
                {
                    Message = $"Không tìm thấy sản phẩm có ID = {id}"
                });
            }

            return Ok(product);
        }
        //Get: api/products/featured
        // Lấy danh sách sản phẩm nổi bật
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedProducts()
        {
            var products = await _context.Products
                .Where(p => p.IsFeatured)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,
                    p.ThumbnailUrl,
                    ImageUrl = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }
        //Get: api/products/search?keyword=abc
        // Tìm kiếm sản phẩm theo keyword
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {
            var products = await _context.Products
                .Where(p => p.ProductName.Contains(keyword))
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    Price = p.SalePrice ?? p.Price,
                    OriginalPrice = p.Price,
                    p.ThumbnailUrl,
                    ImageUrl = p.ProductImages
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }

    }
}