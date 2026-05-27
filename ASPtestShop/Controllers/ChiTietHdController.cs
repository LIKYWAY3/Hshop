using ASPtestShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChiTietHdController : ControllerBase
    {
        private readonly Hshop2023Context _context;

        // Inject DbContext vào trong Controller
        public ChiTietHdController(Hshop2023Context context)
        {
            _context = context;
        }

        // API lấy toàn bộ danh sách chi tiết hóa đơn
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Lấy 20 bản ghi đầu tiên để test tránh ngập dữ liệu
                var data = await _context.ChiTietHds
                    .Take(20)
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi truy vấn database: {ex.Message}");
            }
        }
    }
}