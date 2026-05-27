using Microsoft.AspNetCore.Identity;

namespace ASPtestShop.Data.Entities
{
    public class Cart : BaseEntity
    {
        public int CartId { get; set; }

        public string? UserId { get; set; }

        public string? SessionId { get; set; }

        public User? User { get; set; }


        public ICollection<CartItem> CartItems { get; set; }
            = new List<CartItem>();
    }
}