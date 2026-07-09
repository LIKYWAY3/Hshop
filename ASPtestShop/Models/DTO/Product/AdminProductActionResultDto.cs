namespace ASPtestShop.Models.DTO.Product
{
    // DTO trả kết quả khi Admin create/update/delete product
    public class AdminProductActionResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public AdminProductResultDto? Product { get; set; }
    }
}