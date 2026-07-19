using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Identity;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminAuthService : IAdminAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminAuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AdminLoginResultDto> LoginAsync(AdminLoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.EmailOrUserName);

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUserName);
            }

            if (user == null)
            {
                return new AdminLoginResultDto
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }

            var passwordOk = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordOk)
            {
                return new AdminLoginResultDto
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                return new AdminLoginResultDto
                {
                    Success = false,
                    Message = "Tài khoản này không có quyền Admin"
                };
            }

            return new AdminLoginResultDto
            {
                Success = true,
                Message = "Đăng nhập Admin thành công",
                UserId = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? ""
            };
        }
    }
}