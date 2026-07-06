using ASPtestShop.Models.DTO.Payment;

using ASPtestShop.Services.PaymentProviders;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    // Provider xử lý thanh toán COD
    public class CodPaymentProvider : IPaymentProvider
    {
        public string PaymentMethod => "COD";

        public Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            // COD không cần gọi API bên ngoài
            // Chỉ tạo Payment với trạng thái Unpaid
            var result = new CreatePaymentResultDto
            {
                IsSuccess = true,
                Message = "Tạo thanh toán COD thành công",
                PaymentMethod = "COD",
                PaymentStatus = "Unpaid",
                PaymentUrl = null,
                TransactionCode = null,
                RequiresRedirect = false
            };

            return Task.FromResult(result);
        }
    }
}