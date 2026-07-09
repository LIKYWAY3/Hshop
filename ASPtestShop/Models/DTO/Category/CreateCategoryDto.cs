using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Category
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        // Không nhập cũng được, service sẽ tự tạo từ CategoryName
        [MaxLength(150)]
        public string? Slug { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}