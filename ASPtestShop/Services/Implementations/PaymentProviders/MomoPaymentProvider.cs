using ASPtestShop.Models.DTO.Payment;

using ASPtestShop.Services.PaymentProviders;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    // Khung MoMo Provider
    // Hiện tại chưa gọi API thật
    // Sau này có PartnerCode, AccessKey, SecretKey, ReturnUrl, NotifyUrl thì gắn vào đây
    public class MomoPaymentProvider : IPaymentProvider
    {
        public string PaymentMethod => "MOMO";

        public Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var result = new CreatePaymentResultDto
            {
                IsSuccess = false,
                Message = "MoMo chưa được cấu hình. Hiện tại chỉ hỗ trợ COD.",
                PaymentMethod = "MOMO",
                PaymentStatus = "Pending",
                PaymentUrl = null,
                TransactionCode = null,
                RequiresRedirect = true
            };

            return Task.FromResult(result);
        }
    }
}