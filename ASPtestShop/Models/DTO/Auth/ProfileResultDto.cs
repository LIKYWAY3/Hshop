namespace ASPtestShop.Models.DTO.Auth
{
    // DTO trả thông tin profile lấy từ JWT token
    public class ProfileResultDto
    {
        public string Message { get; set; } = string.Empty;

        public string? UserId { get; set; }

        public string? UserName { get; set; }
    }
}