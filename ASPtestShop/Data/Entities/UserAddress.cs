using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Data.Entities
{
    public class UserAddress
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string SpecificAddress { get; set; } = null!;

        public bool IsDefault { get; set; } 
    }
}