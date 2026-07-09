namespace ASPtestShop.Models.DTO.Category
{
    public class UpdateCategoryDto
    {
        public string? CategoryName { get; set; }

        public string? Slug { get; set; }

        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; }
    }
}