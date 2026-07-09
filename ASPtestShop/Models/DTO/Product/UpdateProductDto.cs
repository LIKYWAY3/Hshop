namespace ASPtestShop.Models.DTO.Product

{
    // DTO dùng khi Admin sửa sản phẩm
    // Dùng nullable để field nào không gửi thì giữ nguyên
    public class UpdateProductDto
    {
        public int? CategoryId { get; set; }

        public string? ProductName { get; set; }

        public string? Slug { get; set; }

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public decimal? SalePrice { get; set; }

        public int? StockQuantity { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool? IsFeatured { get; set; }

        public bool? IsActive { get; set; }
        public List<ProductImageInputDto>? Images { get; set; }
    }
}