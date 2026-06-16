using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPtestShop.Data.Entities
{
    public class Order : BaseEntity
    {
        public int OrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderCode { get; set; }

        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        [MaxLength(30)]
        public string OrderStatus { get; set; } = "Pending";

        [MaxLength(30)]
        public string PaymentStatus { get; set; } = "Unpaid";

        [MaxLength(30)]
        public string PaymentMethod { get; set; }

        [MaxLength(100)]
        public string ReceiverName { get; set; }

        [MaxLength(20)]
        public string ReceiverPhone { get; set; }

        [MaxLength(255)]
        public string ShippingAddress { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public User User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public Payment? Payment { get; set; }
    }
}