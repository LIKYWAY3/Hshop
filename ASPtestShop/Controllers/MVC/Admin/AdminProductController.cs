using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.Admin
{
    [Route("admin/products")]
    public class AdminProductController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
