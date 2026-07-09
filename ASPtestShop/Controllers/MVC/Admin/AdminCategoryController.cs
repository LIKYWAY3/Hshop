using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.Admin
{
    [Route("admin/categories")]
    public class AdminCategoryController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}