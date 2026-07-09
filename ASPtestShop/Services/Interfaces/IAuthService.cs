using ASPtestShop.Models.DTO.Auth;

namespace ASPtestShop.Services.Interfaces;

public interface IAuthService
{
    // Đăng ký tài khoản
    Task<AuthResultDto> RegisterAsync(RegisterDto dto);

    // Đăng nhập và tạo JWT token
    Task<AuthResultDto> LoginAsync(LoginDto dto);

    // Lấy thông tin profile từ claims đã đọc ở Controller
    ProfileResultDto GetProfile(string? userId, string? userName);

    // Gửi email quên mật khẩu
    Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordDto dto);

    // Reset mật khẩu
    Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto dto);

    // Logout và làm token cũ mất hiệu lực
    Task<AuthResultDto> LogoutAsync(string userId);
}