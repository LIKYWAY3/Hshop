using ASPtestShop.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.User
{
    [Route("orders")]
    [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
    public class OrderController : Controller
    {
        // GET: /orders/history
        [HttpGet("history")]
        public IActionResult History()
        {
            return View();
        }

        // GET: /orders/detail/5
        [HttpGet("detail/{orderId:int}")]
        public IActionResult Detail(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}