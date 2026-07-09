using ASPtestShop.Models.DTO.Product;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [Route("api/admin/products")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminProductApiController : ControllerBase
    {
        private readonly IAdminProductService _adminProductService;

        public AdminProductApiController(IAdminProductService adminProductService)
        {
            _adminProductService = adminProductService;
        }

        // GET: api/admin/products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _adminProductService.GetProductsAsync();

            return Ok(products);
        }

        // GET: api/admin/products/{productId}
        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _adminProductService.GetProductByIdAsync(productId);

            if (product == null)
            {
                return NotFound(new
                {
                    Message = "Không tìm thấy sản phẩm"
                });
            }

            return Ok(product);
        }

        // POST: api/admin/products
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            var result = await _adminProductService.CreateProductAsync(dto);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(new
            {
                result.Message,
                result.Product
            });
        }

        // PUT: api/admin/products/{productId}
        [HttpPut("{productId:int}")]
        public async Task<IActionResult> UpdateProduct(int productId, UpdateProductDto dto)
        {
            var result = await _adminProductService.UpdateProductAsync(productId, dto);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(new
            {
                result.Message,
                result.Product
            });
        }

        // DELETE: api/admin/products/{productId}
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var result = await _adminProductService.DeleteProductAsync(productId);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(new
            {
                result.Message,
                result.Product
            });
        }
    }
}