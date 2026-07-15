using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.ViewModels.Auth
{
    public class ChangeEmailViewModel
    {
        [Display(Name = "Mật khẩu hiện tại")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu để xác nhận")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Địa chỉ Email mới")]
        [Required(ErrorMessage = "Vui lòng nhập email mới")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        public string? NewEmail { get; set; }
    }
}