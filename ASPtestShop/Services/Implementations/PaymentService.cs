using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    // PaymentService xử lý các nghiệp vụ sau khi payment đã được tạo
    // Ví dụ: lấy thông tin payment, xác nhận COD
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy thông tin thanh toán theo OrderId
        public async Task<PaymentResultDto?> GetPaymentByOrderIdAsync(int orderId, string userId)
        {
            var payment = await _context.Payments
                // Chỉ lấy payment thuộc đơn hàng của user đang đăng nhập
                .Where(p => p.OrderId == orderId && p.Order.UserId == userId)

                // Map sang DTO, không trả entity Payment trực tiếp
                .Select(p => new PaymentResultDto
                {
                    PaymentId = p.PaymentId,
                    OrderId = p.OrderId,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    TransactionCode = p.TransactionCode,
                    PaidAt = p.PaidAt,

                    Order = new PaymentOrderDto
                    {
                        OrderId = p.Order.OrderId,
                        OrderCode = p.Order.OrderCode,
                        FinalAmount = p.Order.FinalAmount,
                        OrderStatus = p.Order.OrderStatus,
                        PaymentStatus = p.Order.PaymentStatus
                    }
                })
                .FirstOrDefaultAsync();

            return payment;
        }

        // Xác nhận thanh toán COD
        public async Task<bool> ConfirmCodPaymentAsync(int orderId, string userId)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p =>
                    p.OrderId == orderId &&
                    p.Order.UserId == userId);

            // Không tìm thấy payment hoặc không phải đơn của user này
            if (payment == null)
            {
                return false;
            }

            // Chỉ cho xác nhận thanh toán COD
            if (!string.Equals(payment.PaymentMethod, "COD", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Nếu đã thanh toán rồi thì không xác nhận lại
            if (string.Equals(payment.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Cập nhật payment
            payment.PaymentStatus = "Paid";
            payment.PaidAt = DateTime.Now;

            // Cập nhật trạng thái thanh toán của order
            payment.Order.PaymentStatus = "Paid";

            await _context.SaveChangesAsync();

            return true;
        }
    }
}