using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.User
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}