using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Auth;

namespace ASPtestShop.Services.Interfaces.Admin
{
    public interface IAdminAuthService
    {
        Task<AdminLoginResultDto> LoginAsync(AdminLoginViewModel model);
    }
}