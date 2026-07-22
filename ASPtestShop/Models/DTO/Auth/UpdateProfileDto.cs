using Microsoft.AspNetCore.Http;

namespace ASPtestShop.Models.DTO.Auth
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }
}