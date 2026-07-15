using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Auth;

namespace ASPtestShop.Services.Interfaces.User
{
    public interface IUserAuthService
    {
        Task<UserLoginResultDto> LoginAsync(LoginViewModel model);

        Task<UserRegisterResultDto> RegisterAsync(RegisterViewModel model);

        Task<AuthResultDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    }
}