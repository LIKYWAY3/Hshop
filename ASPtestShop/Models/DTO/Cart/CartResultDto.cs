namespace ASPtestShop.Models.DTO.Cart
{
    public class CartResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? ErrorCode { get; set; }

        public int? CartId { get; set; }

        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

        public decimal TotalAmount { get; set; }
    }
}