namespace ASPtestShop.Models.DTO.Auth
{
    public class ForgotPasswordResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        // Tạm thời trả token ra Postman để test.
        // Sau này làm gửi email thật thì không trả token ra response nữa.
        public string? ResetToken { get; set; }
    }
}