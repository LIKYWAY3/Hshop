using System.Security.Claims;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            //xác thực người dùng bản này để bắt chuỗi rỗng nếu chưa đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }
            // Lấy giỏ hàng của người dùng từ cơ sở dữ liệu
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(ci => ci.UserId == userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest(new
                {
                    Message = "Giỏ hàng trống"
                });
            }
            //tính tổng tiền
            var totalAmount = cart.CartItems.Sum(item => item.Quantity * item.Product.Price);
            var discountAmount = 0m;
            var shippingFee = 0m;
            var finalAmount = totalAmount - discountAmount + shippingFee;
            // Nếu phương thức thanh toán không được cung cấp, mặc định là "COD"
            var paymentMethod = string.IsNullOrWhiteSpace(checkoutDto.PaymentMethod) ? "COD" : checkoutDto.PaymentMethod;
            // Bắt đầu một giao dịch để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();

            var order = new Order
            {
                OrderCode = "OD" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),

                UserId = userId,

                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                ShippingFee = shippingFee,
                FinalAmount = finalAmount,

                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                PaymentMethod = paymentMethod,

                ReceiverName = checkoutDto.ReceiverName,
                ReceiverPhone = checkoutDto.ReceiverPhone,
                ShippingAddress = checkoutDto.ShippingAddress,
                Note = checkoutDto.Note
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItems = cart.CartItems.Select(item => new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = item.ProductId,

                ProductNameSnapshot = item.Product.ProductName,
                UnitPrice = item.Product.Price,
                Quantity = item.Quantity,
                LineTotal = item.Quantity * item.Product.Price
            }).ToList();

            _context.OrderItems.AddRange(orderItems);

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(new
            {
                Message = "Đặt hàng thành công",
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                FinalAmount = order.FinalAmount,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod
            });
        }

        //Lịch sử đơn hàng
        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory()
        {
            //xác thực người dùng bản này để bắt chuỗi rỗng nếu chưa đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }
            // Lấy danh sách đơn hàng của người dùng từ cơ sở dữ liệu
            var orders = await _context.Orders
            .Where(o => o.UserId == userId) // Lọc các đơn hàng theo UserId
            .Include(o => o.OrderItems)     // Bao gồm các OrderItems liên quan đến mỗi đơn hàng
            .OrderByDescending(o => o.CreatedAt) // Sắp xếp các đơn hàng theo ngày tạo, mới nhất trước

            // Chọn các trường cần thiết từ mỗi đơn hàng và các OrderItems liên quan
            .Select(o => new OrderHistoryDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                TotalAmount = o.TotalAmount,
                DiscountAmount = o.DiscountAmount,
                ShippingFee = o.ShippingFee,
                FinalAmount = o.FinalAmount,
                OrderStatus = o.OrderStatus,
                PaymentStatus = o.PaymentStatus,
                PaymentMethod = o.PaymentMethod,
                ReceiverName = o.ReceiverName,
                ReceiverPhone = o.ReceiverPhone,
                ShippingAddress = o.ShippingAddress,
                Note = o.Note,
                CreatedAt = o.CreatedAt,

                Items = o.OrderItems.Select(oi => new OrderHistoryItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductNameSnapshot = oi.ProductNameSnapshot,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    LineTotal = oi.LineTotal
                }).ToList()
            }).ToListAsync();

            if (!orders.Any())
            {
                return Ok(new
                {
                    Message = "Bạn chưa có đơn hàng nào",
                    Orders = orders
                });
            }

            return Ok(new
            {
                Message = "Lấy lịch sử đơn hàng thành công",
                Orders = orders
            });
        }

        // Lấy chi tiết lịch sử đơn hàng
        [Authorize]
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }
            // Lấy chi tiết đơn hàng dựa trên orderId và userId
            var order = await _context.Orders
                .Where(o => o.OrderId == orderId && o.UserId == userId)
                .Select(o => new OrderHistoryDto
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount,
                    ShippingFee = o.ShippingFee,
                    FinalAmount = o.FinalAmount,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    ShippingAddress = o.ShippingAddress,
                    Note = o.Note,
                    CreatedAt = o.CreatedAt,
                    // Lấy danh sách các OrderItems liên quan đến đơn hàng
                    Items = o.OrderItems.Select(oi => new OrderHistoryItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductNameSnapshot = oi.ProductNameSnapshot,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        LineTotal = oi.LineTotal
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound(new
                {
                    Message = "Đơn hàng không tồn tại hoặc bạn không có quyền truy cập"
                });
            }
            return Ok(new
            {
                Message = "Lấy chi tiết đơn hàng thành công",
                OrderDetails = order
            });
        }
    }
}