namespace ASPtestShop.Models.DTO.Product
{
    // DTO trả ảnh sản phẩm ra ngoài API
    public class ProductImageDto
    {
        public int ProductImageId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }

        public int SortOrder { get; set; }
    }
}