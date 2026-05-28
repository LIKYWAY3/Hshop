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
    }
}
