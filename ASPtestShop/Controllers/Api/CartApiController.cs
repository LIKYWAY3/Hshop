using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class CartApiController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            // kiểm tra xem người dùng đã đăng nhập chưa, nếu chưa thì trả về lỗi
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = "Bạn chưa đăng nhập" });
            }

            // kiểm tra xem sản phẩm có tồn tại không, nếu không thì trả về lỗi
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);
            if (product == null)
            {
                return NotFound(new { Message = "Sản phẩm không tồn tại" });
            }

            //Tìm giỏ hàng của UserId, nếu chưa có thì tạo mới
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            //Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var cartIten = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == dto.ProductId);
            if (cartIten != null)
            {
                cartIten.Quantity += dto.Quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.SalePrice ?? product.Price
                });
            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Thêm vào giỏ hàng thành công."
            });
        }
    }
}