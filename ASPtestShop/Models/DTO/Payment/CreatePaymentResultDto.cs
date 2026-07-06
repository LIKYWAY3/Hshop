namespace ASPtestShop.Models.DTO.Payment
{
    // DTO kết quả sau khi tạo thanh toán
    public class CreatePaymentResultDto
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        // Dùng cho MoMo/ZaloPay/VNPay sau này
        // COD thì null
        public string? PaymentUrl { get; set; }

        // Mã giao dịch từ cổng thanh toán
        // COD thì null
        public string? TransactionCode { get; set; }

        // True nếu cần chuyển user qua trang thanh toán online
        // COD thì false
        public bool RequiresRedirect { get; set; }
    }
}