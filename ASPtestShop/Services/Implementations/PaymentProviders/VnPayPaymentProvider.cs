using ASPtestShop.Models.DTO.Payment;

using ASPtestShop.Services.PaymentProviders;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    // Khung VNPay Provider
    // Hiện tại chưa gọi API thật
    public class VnPayPaymentProvider : IPaymentProvider
    {
        public string PaymentMethod => "VNPAY";

        public Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var result = new CreatePaymentResultDto
            {
                IsSuccess = false,
                Message = "VNPay chưa được cấu hình. Hiện tại chỉ hỗ trợ COD.",
                PaymentMethod = "VNPAY",
                PaymentStatus = "Pending",
                PaymentUrl = null,
                TransactionCode = null,
                RequiresRedirect = true
            };

            return Task.FromResult(result);
        }
    }
}