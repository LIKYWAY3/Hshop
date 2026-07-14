namespace ASPtestShop.Models.DTO.Order
{
    public class AdminOrderItemDto
    {
        public int OrderItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductNameSnapshot { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal LineTotal { get; set; }
    }
}