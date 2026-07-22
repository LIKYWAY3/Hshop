using Microsoft.AspNetCore.Identity;

namespace ASPtestShop.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }
        public string? Gender { get; set; }
    }
}