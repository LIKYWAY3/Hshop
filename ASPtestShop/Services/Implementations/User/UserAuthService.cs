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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserAuthService(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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
                Address = model.Address,
                Gender = model.Gender
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
        public async Task<AuthResultDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Message = "Không tìm thấy người dùng!" };
            }

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.Gender = dto.Gender;

            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "Avatars");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.AvatarFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.AvatarFile.CopyToAsync(fileStream);
                }

                user.AvatarUrl = "/Uploads/Avatars/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new AuthResultDto { Success = true, Message = "Cập nhật hồ sơ thành công!" };
            }

            return new AuthResultDto
            {
                Success = false,
                Message = "Cập nhật thất bại",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }
    }
}