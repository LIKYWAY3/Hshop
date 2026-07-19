using System.Security.Claims;
using ASPtestShop.Auth;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.User
{
    [Route("checkout")]
    [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;

        public CheckoutController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /checkout
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge(UserCookieAuth.Scheme);
            }

            var hasCartItems =
                await _cartService.HasCartItemsAsync(userId);

            if (!hasCartItems)
            {
                TempData["ErrorMessage"] =
                    "Giỏ hàng đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";

                return RedirectToAction("Index", "Cart");
            }

            return View();
        }

        // GET: /checkout/success
        [HttpGet("success")]
        public IActionResult Success(string? orderCode = null)
        {
            ViewBag.OrderCode = orderCode;
            return View();
        }
    }
}