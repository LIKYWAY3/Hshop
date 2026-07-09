using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryApiController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryApiController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //===============================GET CATEGORIES======================================
        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();

            return Ok(categories);
        }

        //===============================GET CATEGORY BY ID======================================
        // GET: api/categories/{categoryId}
        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                return NotFound(new
                {
                    Message = $"Không tìm thấy danh mục có ID = {categoryId}"
                });
            }

            return Ok(category);
        }

        //===============================GET PARENT CATEGORIES======================================
        // GET: api/categories/parents
        [HttpGet("parents")]
        public async Task<IActionResult> GetParentCategories()
        {
            var categories = await _categoryService.GetParentCategoriesAsync();

            return Ok(categories);
        }

        //===============================GET SUBCATEGORIES BY PARENT CATEGORYID======================================
        // GET: api/categories/{parentCategoryId}/subcategories
        [HttpGet("{parentCategoryId:int}/subcategories")]
        public async Task<IActionResult> GetSubCategories(int parentCategoryId)
        {
            var categories = await _categoryService.GetSubCategoriesAsync(parentCategoryId);

            return Ok(categories);
        }
    }
}