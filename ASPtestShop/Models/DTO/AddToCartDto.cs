namespace ASPtestShop.Models.DTO
{
    public class AddToCartDto
    {
        //public string? UserId { get; set; } không cần thiết vì sẽ lấy Userid từ token
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}