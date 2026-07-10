using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.ViewModels.Auth
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email hoặc username")]
        public string EmailOrUserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}