using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.ViewModels.Profile
{
    public class AddAddressViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ cụ thể")]
        public string SpecificAddress { get; set; } = null!;

        public bool IsDefault { get; set; } 
    }
}