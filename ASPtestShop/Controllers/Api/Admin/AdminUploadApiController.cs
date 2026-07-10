using ASPtestShop.Auth;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [Route("api/admin/uploads")]
    [ApiController]
    [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + AdminCookieAuth.Scheme, Roles = "Admin")]
    public class AdminUploadApiController : ControllerBase
    {
        private readonly IAdminUploadService _adminUploadService;

        public AdminUploadApiController(IAdminUploadService adminUploadService)
        {
            _adminUploadService = adminUploadService;
        }

        // POST: api/admin/uploads/product-image
        [HttpPost("product-image")]
        public async Task<IActionResult> UploadProductImage(IFormFile file)
        {
            var result = await _adminUploadService.UploadProductImageAsync(file);

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
                result.ImageUrl
            });
        }
    }
}