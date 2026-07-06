using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Data.Entities
{
    public class ChatAttachment : BaseEntity
    {
        public Guid ChatAttachmentId { get; set; }

        public Guid ChatMessageId { get; set; }

        public EnumChatMessageType AttachmentType { get; set; }
            = EnumChatMessageType.Image;

        [Required]
        [MaxLength(1000)]
        public string Url { get; set; } = null!;

        [MaxLength(255)]
        public string? FileName { get; set; }

        [MaxLength(100)]
        public string? ContentType { get; set; }

        public long FileSize { get; set; }

        public virtual ChatMessage ChatMessage { get; set; } = null!;
    }
}