namespace ASPtestShop.Models.DTO.Auth
{
    public class UserProfileDto
    {
        public string Id { get; set; } = null!;
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? AvatarUrl { get; set; }
    }
}