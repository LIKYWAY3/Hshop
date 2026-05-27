namespace ASPtestShop.Data.Entities
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string Role { get; set; } = "Guest";
    
        public string? AvatarUrl { get; set; }

        // Navigation Properties
        public ICollection<Cart> Carts { get; set; }
            = new List<Cart>();

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();

        public ICollection<Review> Reviews { get; set; }
            = new List<Review>();
    }
}