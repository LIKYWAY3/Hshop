namespace ASPtestShop.Models.DTO.Category
{
    // DTO dùng để trả dữ liệu Category ra ngoài API
    // Không trả trực tiếp Entity Category để tránh dư navigation như Products, SubCategories
    public class CategoryResultDto
    {
        // Mã danh mục
        public int CategoryId { get; set; }

        // Tên danh mục
        public string CategoryName { get; set; } = string.Empty;

        // Đường dẫn SEO
        public string Slug { get; set; } = string.Empty;

        // Mô tả danh mục
        public string? Description { get; set; }

        // Mã danh mục cha, nếu là danh mục gốc thì null
        public int? ParentCategoryId { get; set; }

        // Tên danh mục cha
        public string? ParentCategoryName { get; set; }

        // Số lượng sản phẩm trong danh mục
        public int ProductCount { get; set; }

        // Số lượng danh mục con
        public int SubCategoryCount { get; set; }

        // Trạng thái hoạt động
        public bool IsActive { get; set; }
    }
}