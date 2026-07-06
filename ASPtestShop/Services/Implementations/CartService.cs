using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Cart;
using ASPtestShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartResultDto> AddToCartAsync(string userId, AddToCartDto dto)
        {
            if (dto == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorCode = "BadRequest",
                    Message = "Dữ liệu không hợp lệ"
                };
            }

            if (dto.Quantity <= 0)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorCode = "BadRequest",
                    Message = "Số lượng phải lớn hơn 0"
                };
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);
            if (product == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorCode = "NotFound",
                    Message = "Sản phẩm không tồn tại"
                };
            }

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

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci =>
                    ci.CartId == cart.CartId &&
                    ci.ProductId == dto.ProductId);

            if (cartItem != null)
            {
                cartItem.Quantity += dto.Quantity;
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

            return new CartResultDto
            {
                Success = true,
                Message = "Thêm vào giỏ hàng thành công."
            };
        }

        public async Task<CartResultDto> GetCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return new CartResultDto
                {
                    Success = true,
                    Message = "Giỏ hàng trống",
                    Items = new List<CartItemDto>(),
                    TotalAmount = 0
                };
            }

            var items = cart.CartItems.Select(ci => new CartItemDto
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                ProductName = ci.Product.ProductName,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                Total = ci.Quantity * ci.UnitPrice
            }).ToList();

            var totalAmount = items.Sum(i => i.Total);

            return new CartResultDto
            {
                Success = true,
                Message = "Lấy giỏ hàng thành công",
                CartId = cart.CartId,
                Items = items,
                TotalAmount = totalAmount
            };
        }

        public async Task<CartResultDto> UpdateCartItemAsync(string userId, UpdateCartItemDto dto)
        {
            if (dto == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorCode = "BadRequest",
                    Message = "Dữ liệu không hợp lệ"
                };
            }

            if (dto.Quantity <= 0)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorCode = "BadRequest",
                    Message = "Số lượng phải lớn hơn 0"
                };
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci =>
                    ci.CartItemId == dto.CartItemId &&
                    ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorCode = "NotFound",
                    Message = "Sản phẩm không tồn tại trong giỏ hàng"
                };
            }

            cartItem.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return new CartResultDto
            {
                Success = true,
                Message = "Cập nhật số lượng sản phẩm trong giỏ hàng thành công."
            };
        }

        public async Task<CartResultDto> RemoveCartItemAsync(string userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci =>
                    ci.CartItemId == cartItemId &&
                    ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return new CartResultDto
                {
                    Success = false,
                    ErrorCode = "NotFound",
                    Message = "Sản phẩm không tồn tại trong giỏ hàng"
                };
            }

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();

            return new CartResultDto
            {
                Success = true,
                Message = "Xóa sản phẩm khỏi giỏ hàng thành công."
            };
        }

        public async Task<CartResultDto> ClearCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return new CartResultDto
                {
                    Success = true,
                    Message = "Giỏ hàng trống",
                    Items = new List<CartItemDto>(),
                    TotalAmount = 0
                };
            }

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return new CartResultDto
            {
                Success = true,
                Message = "Xóa toàn bộ sản phẩm khỏi giỏ hàng thành công."
            };
        }
    }
}