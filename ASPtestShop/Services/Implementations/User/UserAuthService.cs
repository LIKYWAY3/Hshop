using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.Interfaces.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations.User
{
    public class UserAuthService : IUserAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        public UserAuthService(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, HttpClient httpClient, AppDbContext context)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _httpClient = httpClient;
            _context = context;
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

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var defaultAddress = await _context.UserAddresses
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsDefault);

            string addressDisplay = defaultAddress != null ? defaultAddress.SpecificAddress : "Chưa thiết lập";

            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = addressDisplay,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl
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
        public async Task<List<BankAccountDto>> GetBankAccountsAsync(string userId)
        {
            try
            {
                string apiUrl = $"https://api.yourdomain.com/v1/banks/{userId}";

                var banks = await _httpClient.GetFromJsonAsync<List<BankAccountDto>>(apiUrl);

                return banks ?? new List<BankAccountDto>();
            }
            catch (Exception)
            {
                return new List<BankAccountDto>();
            }
        }

        public async Task<AuthResultDto> AddBankAccountAsync(string userId, AddBankAccountViewModel model)
        {
            try
            {
                string apiUrl = $"https://api.yourdomain.com/v1/banks/add";

                var payload = new
                {
                    UserId = userId,
                    BankName = model.BankName,
                    AccountName = model.AccountName.ToUpper(),
                    AccountNumber = model.AccountNumber
                };

                var response = await _httpClient.PostAsJsonAsync(apiUrl, payload);

                if (response.IsSuccessStatusCode)
                {
                    return new AuthResultDto { Success = true, Message = "Thêm tài khoản ngân hàng thành công!" };
                }

                return new AuthResultDto { Success = false, Message = "API từ chối yêu cầu." };
            }
            catch (Exception ex)
            {
                return new AuthResultDto { Success = false, Message = "Lỗi kết nối đến Server API." };
            }
        }
        public async Task<List<UserAddress>> GetUserAddressesAsync(string userId)
        {
            return await _context.UserAddresses
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsDefault)
                .ToListAsync();
        }

        public async Task<AuthResultDto> AddAddressAsync(string userId, AddAddressViewModel model)
        {
            if (model.IsDefault)
            {
                var currentDefaults = await _context.UserAddresses
                    .Where(x => x.UserId == userId && x.IsDefault)
                    .ToListAsync();

                foreach (var addr in currentDefaults)
                {
                    addr.IsDefault = false;
                }
            }

            var newAddress = new UserAddress
            {
                UserId = userId,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                SpecificAddress = model.SpecificAddress,
                IsDefault = model.IsDefault
            };

            _context.UserAddresses.Add(newAddress);
            await _context.SaveChangesAsync();

            return new AuthResultDto { Success = true, Message = "Thêm địa chỉ mới thành công!" };
        }
        public async Task<AuthResultDto> SetDefaultAddressAsync(string userId, int addressId)
        {
            var addresses = await _context.UserAddresses.Where(x => x.UserId == userId).ToListAsync();
            var targetAddress = addresses.FirstOrDefault(x => x.Id == addressId);

            if (targetAddress == null) return new AuthResultDto { Success = false, Message = "Không tìm thấy địa chỉ" };

            foreach (var addr in addresses) { addr.IsDefault = false; }
            targetAddress.IsDefault = true;

            await _context.SaveChangesAsync();
            return new AuthResultDto { Success = true, Message = "Đã thiết lập làm địa chỉ mặc định!" };
        }

        public async Task<AuthResultDto> EditAddressAsync(string userId, EditAddressViewModel model)
        {
            var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == userId);
            if (address == null) return new AuthResultDto { Success = false, Message = "Không tìm thấy địa chỉ" };

            if (model.IsDefault && !address.IsDefault)
            {
                var currentDefaults = await _context.UserAddresses.Where(x => x.UserId == userId && x.IsDefault).ToListAsync();
                foreach (var addr in currentDefaults) { addr.IsDefault = false; }
            }

            address.FullName = model.FullName;
            address.PhoneNumber = model.PhoneNumber;
            address.SpecificAddress = model.SpecificAddress;

            if (!address.IsDefault || model.IsDefault)
            {
                address.IsDefault = model.IsDefault;
            }

            await _context.SaveChangesAsync();
            return new AuthResultDto { Success = true, Message = "Cập nhật địa chỉ thành công!" };
        }
    }
}