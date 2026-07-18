using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.ViewModels.Auth
{
    public class UserAddressViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string SpecificAddress { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public bool IsDefault { get; set; }

        public string MaskedPhoneNumber
        {
            get
            {
                if (string.IsNullOrEmpty(PhoneNumber)) return "";
                if (PhoneNumber.Length > 4)
                {
                    return "********" + PhoneNumber.Substring(PhoneNumber.Length - 3);
                }
                return PhoneNumber;
            }
        }

        public string FullAddress => $"{SpecificAddress}, {Ward}, {District}, {Province}";
    }

    public class AddressPageViewModel
    {
        public List<UserAddressViewModel> Addresses { get; set; } = new List<UserAddressViewModel>();

        public AddAddressInputModel NewAddress { get; set; } = new AddAddressInputModel();
    }

    public class AddAddressInputModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ cụ thể")]
        public string SpecificAddress { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        public string Ward { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public string District { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
        public string Province { get; set; }

        public bool IsDefault { get; set; }
    }
}