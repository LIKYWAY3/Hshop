using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Models.DTO.Payment;

using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IPaymentProviderFactory _paymentProviderFactory;

        public OrderService(AppDbContext context, IPaymentProviderFactory paymentProviderFactory)
        {
            _context = context;
            _paymentProviderFactory = paymentProviderFactory;
        }

        public async Task<CheckoutResultDto> CheckoutAsync(string userId, CheckoutDto checkoutDto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return new CheckoutResultDto
                {
                    Success = false,
                    Message = "Giỏ hàng trống"
                };
            }

            var paymentMethod = string.IsNullOrWhiteSpace(checkoutDto.PaymentMethod)
            ? "COD"
            : checkoutDto.PaymentMethod.Trim().ToUpper();

            IPaymentProvider provider;

            try
            {
                provider = _paymentProviderFactory.GetProvider(paymentMethod);
            }
            catch (NotSupportedException ex)
            {
                return new CheckoutResultDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }

            var totalAmount = cart.CartItems.Sum(item =>
            item.Quantity * (item.Product.SalePrice ?? item.Product.Price)
            );

            var discountAmount = 0m;
            var shippingFee = 0m;
            var finalAmount = totalAmount - discountAmount + shippingFee;

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

            var orderItems = cart.CartItems.Select(item =>
            {
                var unitPrice = item.Product.SalePrice ?? item.Product.Price;

                return new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,

                    ProductNameSnapshot = item.Product.ProductName,
                    UnitPrice = unitPrice,
                    Quantity = item.Quantity,
                    LineTotal = item.Quantity * unitPrice
                };
            }).ToList();

            _context.OrderItems.AddRange(orderItems);

            var paymentResult = await provider.CreatePaymentAsync(new CreatePaymentRequestDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                UserId = userId,
                Amount = order.FinalAmount,
                PaymentMethod = paymentMethod
            });

            if (!paymentResult.IsSuccess)
            {
                await transaction.RollbackAsync();

                return new CheckoutResultDto
                {
                    Success = false,
                    Message = paymentResult.Message
                };
            }

            var payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = paymentResult.PaymentMethod,
                PaymentStatus = paymentResult.PaymentStatus,
                TransactionCode = paymentResult.TransactionCode,
                PaidAt = paymentResult.PaymentStatus == "Paid" ? DateTime.Now : null
            };

            _context.Payments.Add(payment);

            // Cập nhật lại payment status của order theo provider
            order.PaymentMethod = paymentResult.PaymentMethod;
            order.PaymentStatus = paymentResult.PaymentStatus;

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new CheckoutResultDto
            {
                Success = true,
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
            };
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
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
                })
                .ToListAsync();

            return orders;
        }

        public async Task<OrderHistoryDto?> GetOrderDetailAsync(string userId, int orderId)
        {
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

            return order;
        }
    }
}