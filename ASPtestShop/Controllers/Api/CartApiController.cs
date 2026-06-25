using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPtestShop.Models.DTO.Cart;

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

            // kiểm tra xem sản phẩm có tồn tại không
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

        // Lấy thông tin giỏ hàng của người dùng
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = "Bạn chưa đăng nhập" });
            }
            // Lấy giỏ hàng của người dùng, bao gồm các sản phẩm trong giỏ hàng
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return Ok(new
                {
                    Message = "Giỏ hàng trống",
                    Items = new List<object>(),
                    TotalAmount = 0
                });
            }
            var items = cart.CartItems.Select(ci => new
            {
                ci.CartItemId,
                ci.ProductId,
                ci.Product.ProductName,
                ci.Quantity,
                ci.UnitPrice,
                Total = ci.Quantity * ci.UnitPrice
            }).ToList();

            var totalAmount = items.Sum(i => i.Total);

            return Ok(new
            {
                cart.CartId,
                Items = items,
                TotalAmount = totalAmount
            });
        }

        //update số lượng sản phẩm trong giỏ hàng
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = "Bạn chưa đăng nhập" });
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            // Kiểm tra số lượng sản phẩm có hợp lệ không
            if (dto.Quantity <= 0)
            {
                return BadRequest(new
                {
                    Message = "Số lượng phải lớn hơn 0"
                });
            }

            // Kiểm tra giỏ hàng có tồn tại không
            if (cart == null)
            {
                return NotFound(new { Message = "Giỏ hàng không tồn tại" });
            }
            var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci =>
            ci.CartItemId == dto.CartItemId &&
            ci.Cart.UserId == userId);
            // Kiểm tra sản phẩm có tồn tại trong giỏ hàng không
            if (cartItem == null)
            {
                return NotFound(new { Message = "Sản phẩm không tồn tại trong giỏ hàng" });
            }

            cartItem.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật số lượng sản phẩm trong giỏ hàng thành công." });
        }

        // Xóa sản phẩm khỏi giỏ hàng theo cartItemId, chỉ xóa sản phẩm của người dùng hiện tại
        //User bấm Xóa -> DELETE /api/cart/remove/{cartItemId} -> [Authorize] -> Lấy UserId từ JWT -> Tìm CartItem thuộc User đó -> Remove() -> SaveChangesAsync() -> Return Ok
        [Authorize]
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = "Bạn chưa đăng nhập" });
            }
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci =>
                    ci.CartItemId == cartItemId &&
                    ci.Cart.UserId == userId);
            if (cartItem == null)
            {
                return NotFound(new { Message = "Sản phẩm không tồn tại trong giỏ hàng" });
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Xóa sản phẩm khỏi giỏ hàng thành công." });
        }
    }
}