using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string OrderStatus { get; set; } = string.Empty;

        [Required]
        public string PaymentStatus { get; set; } = string.Empty;
    }
}