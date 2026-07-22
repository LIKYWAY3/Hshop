namespace ASPtestShop.Models.DTO.Auth
{
    public class BankAccountDto
    {
        public string BankName { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public string? Branch { get; set; }
    }
}