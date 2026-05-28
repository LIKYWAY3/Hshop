using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("ok");
    }
}