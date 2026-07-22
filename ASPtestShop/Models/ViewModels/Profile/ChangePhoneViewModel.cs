using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.ViewModels.Profile
{
    public class ChangePhoneViewModel
    {
        [Display(Name = "Mật khẩu hiện tại")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu để xác nhận")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Số điện thoại mới")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại mới")]
        [Phone(ErrorMessage = "Định dạng số không hợp lệ")]
        public string? NewPhone { get; set; }
    }
}