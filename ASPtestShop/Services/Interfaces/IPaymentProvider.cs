using ASPtestShop.Models.DTO.Payment;

namespace ASPtestShop.Services.PaymentProviders
{
    // Interface chung cho mọi phương thức thanh toán
    // COD, MoMo, ZaloPay, VNPay đều phải implement interface này
    public interface IPaymentProvider
    {
        // Tên phương thức thanh toán
        // Ví dụ: COD, MOMO, ZALOPAY, VNPAY
        string PaymentMethod { get; }

        // Tạo thanh toán
        Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request);
    }
}