namespace ASPtestShop.Models.DTO.Auth
{
    // DTO dùng để trả kết quả Register/Login
    public class AuthResultDto
    {
        // Thành công hay thất bại
        public bool Success { get; set; }

        // Thông báo trả về cho client
        public string Message { get; set; } = string.Empty;

        // Token chỉ có khi đăng nhập thành công
        public string? Token { get; set; }

        // Danh sách lỗi, dùng cho register thất bại
        public List<string> Errors { get; set; } = new List<string>();
    }
}