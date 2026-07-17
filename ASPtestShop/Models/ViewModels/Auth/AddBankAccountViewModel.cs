using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.ViewModels.Auth
{
    public class AddBankAccountViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn ngân hàng")]
        public string BankName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản")]
        public string AccountNumber { get; set; } = null!;

        public string? Branch { get; set; }
    }
}