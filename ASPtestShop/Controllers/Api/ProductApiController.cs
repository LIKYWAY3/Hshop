using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    [Route("api/products")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductService _productService;

        // Inject IProductService
        // Controller không dùng AppDbContext trực tiếp nữa
        public ProductApiController(IProductService productService)
        {
            _productService = productService;
        }

        //===============================GET PRODUCTS======================================
        // GET: api/products
        // Lấy danh sách sản phẩm
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            // Gọi ProductService xử lý logic lấy dữ liệu
            var products = await _productService.GetProductsAsync();

            // Trả kết quả về client
            return Ok(products);
        }

        //===============================GET PRODUCTS BY CATEGORY======================================
        // GET: api/products/category/{categoryId}
        // Lấy danh sách sản phẩm từ categoryId
        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            // Gọi service lấy danh sách sản phẩm theo category
            var products = await _productService.GetProductsByCategoryAsync(categoryId);

            return Ok(products);
        }

        //===============================GET PRODUCT BY ID======================================
        // GET: api/products/{id}
        // Lấy chi tiết 1 sản phẩm
        // Dùng {id:int} để tránh đụng route với "featured" hoặc "search"
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            // Gọi service tìm sản phẩm theo id
            var product = await _productService.GetProductByIdAsync(id);

            // Nếu service trả về null nghĩa là không tìm thấy sản phẩm
            if (product == null)
            {
                return NotFound(new
                {
                    Message = $"Không tìm thấy sản phẩm có ID = {id}"
                });
            }

            return Ok(product);
        }

        //===============================GET FEATURED PRODUCTS======================================
        // GET: api/products/featured
        // Lấy danh sách sản phẩm nổi bật
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedProducts()
        {
            // Gọi service lấy sản phẩm nổi bật
            var products = await _productService.GetFeaturedProductsAsync();

            return Ok(products);
        }

        //===============================SEARCH PRODUCTS======================================
        // GET: api/products/search?keyword=abc
        // Tìm kiếm sản phẩm theo keyword
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {
            // Nếu keyword rỗng thì trả BadRequest
            // Tránh lỗi ProductName.Contains(null)
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new
                {
                    Message = "Vui lòng nhập từ khóa tìm kiếm"
                });
            }

            // Gọi service tìm kiếm sản phẩm
            var products = await _productService.SearchProductsAsync(keyword);

            return Ok(products);
        }
    }
}