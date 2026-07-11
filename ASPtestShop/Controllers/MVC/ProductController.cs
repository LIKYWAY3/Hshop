using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC
{
    public class ProductController : Controller
    {
        // Khi người dùng vào link: /Product/Detail/5
        [HttpGet("Product/Detail/{id:int}")]
        public IActionResult Detail(int id)
        {
            // Bắn cái ID này sang bên giao diện (View) để Javascript biết đường mà lấy
            ViewBag.ProductId = id;
            return View();
        }
    }
}