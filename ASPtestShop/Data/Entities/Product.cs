using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPtestShop.Data.Entities
{
    public class Product : BaseEntity
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        [Required] 
        [MaxLength(200)]
        public string ProductName { get; set; }

        [Required]
        [MaxLength(250)]
        public string Slug { get; set; }

        [MaxLength(500)]
        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalePrice { get; set; }

        public int StockQuantity { get; set; }

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        public bool IsFeatured { get; set; } = false;

        public Category Category { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }
            = new List<ProductImage>();

        public ICollection<CartItem> CartItems { get; set; }
            = new List<CartItem>();

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public ICollection<Review> Reviews { get; set; }
            = new List<Review>();
    }
}