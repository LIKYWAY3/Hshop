using ASPtestShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ASPtestShop.Controllers.Api
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryApiController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    ImageUrl = c.Products
                        .OrderBy(p => p.ProductId)
                        .SelectMany(p => p.ProductImages)
                        .OrderBy(img => img.SortOrder)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();
            return Ok(categories);
        }











    }
}
