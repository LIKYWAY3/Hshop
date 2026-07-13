using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ASPtestShop.Models.ViewModels.Auth
{
    public class UserProfileViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [Display(Name = "Tên đăng nhập (Không thể đổi)")]
        public string? UserName { get; set; }

        [Display(Name = "Địa chỉ Email")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Định dạng Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }

        // Biến này để hứng file ảnh mới khi người dùng bấm nút Upload từ máy tính
        [Display(Name = "Tải ảnh đại diện mới")]
        public IFormFile? AvatarFile { get; set; }
    }
}