using ASPtestShop.Models.DTO.Payment;

using ASPtestShop.Models.DTO.Payment;

using ASPtestShop.Services.PaymentProviders;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    // Khung ZaloPay Provider
    // Hiện tại chưa gọi API thật
    public class ZaloPayPaymentProvider : IPaymentProvider
    {
        public string PaymentMethod => "ZALOPAY";

        public Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var result = new CreatePaymentResultDto
            {
                IsSuccess = false,
                Message = "ZaloPay chưa được cấu hình. Hiện tại chỉ hỗ trợ COD.",
                PaymentMethod = "ZALOPAY",
                PaymentStatus = "Pending",
                PaymentUrl = null,
                TransactionCode = null,
                RequiresRedirect = true
            };

            return Task.FromResult(result);
        }
    }
}