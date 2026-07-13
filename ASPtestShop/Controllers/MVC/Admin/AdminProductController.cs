using ASPtestShop.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.Admin
{
    [Route("admin/products")]
    public class AdminProductController : Controller
    {
        [Authorize(AuthenticationSchemes = AdminCookieAuth.Scheme, Roles = "Admin")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}