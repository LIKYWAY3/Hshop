using ASPtestShop.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Order
{
    public class CheckoutDto
    {
        [Required]
        [MaxLength(100)]
        public string ReceiverName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Note { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}