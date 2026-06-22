using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}