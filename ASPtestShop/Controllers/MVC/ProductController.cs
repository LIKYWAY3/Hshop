using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.User
{
    public class ProductController : Controller
    {
        [HttpGet("Product/Detail/{id:int}")]
        public IActionResult Detail(int id)
        {
            ViewBag.ProductId = id;
            return View();
        }
    }
}