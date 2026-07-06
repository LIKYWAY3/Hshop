namespace ASPtestShop.Models.DTO.Payment
{
    // DTO dùng để gửi thông tin order qua PaymentProvider
    public class CreatePaymentRequestDto
    {
        public int OrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
    }
}