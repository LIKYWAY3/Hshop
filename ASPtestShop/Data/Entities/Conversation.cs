using ASPtestShop.Data.Entities;
using System.ComponentModel.DataAnnotations;
namespace ASPtestShop.Data.Entities
{
    public class Conversation : BaseEntity
    {
        public Guid ConversationId { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = null!;

        [MaxLength(200)]
        public string? Title { get; set; }

        public bool IsClosed { get; set; } = false;

        public DateTime? EndedAt { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
            = new List<ChatMessage>();

    }
}