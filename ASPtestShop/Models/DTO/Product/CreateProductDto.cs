using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Product
{
    // DTO dùng khi Admin thêm sản phẩm
    public class CreateProductDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        // Nếu không truyền Slug thì Service sẽ tự tạo từ ProductName
        [MaxLength(250)]
        public string? Slug { get; set; }

        [MaxLength(500)]
        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public decimal? SalePrice { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool IsFeatured { get; set; } = false;
        public List<ProductImageInputDto> Images { get; set; } = new List<ProductImageInputDto>();
    }
}