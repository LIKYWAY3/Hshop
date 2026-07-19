using ASPtestShop.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.Admin
{
    [Route("admin/orders")]
    [Authorize(AuthenticationSchemes = AdminCookieAuth.Scheme, Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        // GET: /admin/orders
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /admin/orders/detail/5
        [HttpGet("detail/{orderId:int}")]
        public IActionResult Detail(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}