using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Data.Entities
{
    public class Review : BaseEntity
    {
        public int ReviewId { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public bool IsApproved { get; set; } = false;

        public Product Product { get; set; }

        public User User { get; set; }
    }
}