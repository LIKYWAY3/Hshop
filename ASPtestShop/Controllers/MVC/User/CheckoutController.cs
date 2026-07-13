using ASPtestShop.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.User
{
    [Route("checkout")]
    [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
    public class CheckoutController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("success")]
        public IActionResult Success(string? orderCode = null)
        {
            ViewBag.OrderCode = orderCode;
            return View();
        }
    }
}