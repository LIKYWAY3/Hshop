namespace ASPtestShop.Models.DTO.Payment
{
    // DTO trả thông tin thanh toán ra ngoài API
    // Không trả trực tiếp Entity Payment để tránh controller đụng tới Payment.Order
    public class PaymentResultDto
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public string? TransactionCode { get; set; }

        public DateTime? PaidAt { get; set; }

        public PaymentOrderDto Order { get; set; } = new PaymentOrderDto();
    }

    // DTO nhỏ chứa thông tin đơn hàng trong payment
    public class PaymentOrderDto
    {
        public int OrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public decimal FinalAmount { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;
    }
}