using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Data.Entities
{
    public class ProductImage : BaseEntity
    {
        public int ProductImageId { get; set; }

        public int ProductId { get; set; } 

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }

        public bool IsPrimary { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        public Product Product { get; set; }
    }
}