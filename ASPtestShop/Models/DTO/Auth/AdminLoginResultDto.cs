namespace ASPtestShop.Models.DTO.Auth
{
    public class AdminLoginResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}