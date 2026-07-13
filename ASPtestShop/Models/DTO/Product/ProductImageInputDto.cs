using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Product
{
    // DTO admin gửi lên khi thêm/sửa ảnh sản phẩm
    public class ProductImageInputDto
    {
        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; } = false;

        public int SortOrder { get; set; } = 0;
    }
}