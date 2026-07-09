namespace ASPtestShop.Models.DTO.Category
{
    public class AdminCategoryActionResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public AdminCategoryResultDto? Category { get; set; }
    }
}