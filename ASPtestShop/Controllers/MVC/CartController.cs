using ASPtestShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ASPtestShop.Auth; // <-- Thêm dòng này để gọi thư viện Auth của nhóm

namespace ASPtestShop.Controllers
{
    public class CartController : Controller
    {
        [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
        public IActionResult Cart()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}