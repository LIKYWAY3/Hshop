using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Data.Entities
{
    public class Payment : BaseEntity
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        [MaxLength(30)]
        public string PaymentMethod { get; set; }

        [MaxLength(30)]
        public string PaymentStatus { get; set; }

        [MaxLength(100)]
        public string? TransactionCode { get; set; }

        public DateTime? PaidAt { get; set; }

        public Order Order { get; set; }
    }
}