namespace ASPtestShop.Models.DTO.Product
{
    // DTO dùng cho chi tiết sản phẩm:
    // GET /api/products/{id}
    public class ProductDetailDto
    {
        // Mã sản phẩm
        public int ProductId { get; set; }

        // Tên sản phẩm
        public string ProductName { get; set; } = string.Empty;

        // Mô tả chi tiết sản phẩm
        public string? Description { get; set; }

        // Giá bán thực tế
        // Nếu SalePrice có giá trị thì lấy SalePrice
        // Nếu SalePrice null thì lấy Price gốc
        public decimal Price { get; set; }

        // Giá gốc
        public decimal OriginalPrice { get; set; }

        // Số lượng tồn kho
        public int StockQuantity { get; set; }

        // Ảnh đại diện
        public string? ThumbnailUrl { get; set; }

        // Danh sách ảnh phụ của sản phẩm
        public List<string> Images { get; set; } = new List<string>();
    }
}