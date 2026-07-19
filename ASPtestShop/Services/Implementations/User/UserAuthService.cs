using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.Interfaces.User;
using Microsoft.AspNetCore.Identity;

namespace ASPtestShop.Services.Implementations.User
{
    public class UserAuthService : IUserAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserLoginResultDto> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.EmailOrUserName);

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUserName);
            }

            if (user == null)
            {
                return new UserLoginResultDto
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }

            var passwordOk = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordOk)
            {
                return new UserLoginResultDto
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                return new UserLoginResultDto
                {
                    Success = false,
                    Message = "Tài khoản Admin vui lòng đăng nhập tại trang quản trị"
                };
            }

            return new UserLoginResultDto
            {
                Success = true,
                Message = "Đăng nhập thành công",
                UserId = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FullName = user.FullName ?? ""
            };
        }

        public async Task<UserRegisterResultDto> RegisterAsync(RegisterViewModel model)
        {
            var emailExists = await _userManager.FindByEmailAsync(model.Email);

            if (emailExists != null)
            {
                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = "Email này đã được sử dụng"
                };
            }

            var usernameExists = await _userManager.FindByNameAsync(model.UserName);

            if (usernameExists != null)
            {
                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = "Username này đã được sử dụng"
                };
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));

                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = errors
                };
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return new UserRegisterResultDto
            {
                Success = true,
                Message = "Đăng ký tài khoản thành công"
            };
        }

        public async Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new ForgotPasswordResultDto
                {
                    Success = true,
                    Message = "Nếu email tồn tại trong hệ thống, link đặt lại mật khẩu sẽ được tạo."
                };
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return new ForgotPasswordResultDto
            {
                Success = true,
                Message = "Đã tạo link đặt lại mật khẩu.",
                Email = user.Email ?? model.Email,
                ResetToken = resetToken
            };
        }

        public async Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new ResetPasswordResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy tài khoản"
                };
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.ResetToken,
                model.NewPassword
            );

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));

                return new ResetPasswordResultDto
                {
                    Success = false,
                    Message = errors
                };
            }

            await _userManager.UpdateSecurityStampAsync(user);

            return new ResetPasswordResultDto
            {
                Success = true,
                Message = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại."
            };
        }
    }
}