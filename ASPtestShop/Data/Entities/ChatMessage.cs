using ASPtestShop.Data.Entities;

using System.ComponentModel.DataAnnotations;
namespace ASPtestShop.Data.Entities
{
    public class ChatMessage : BaseEntity
    {
        public Guid ChatMessageId { get; set; }
        public Guid ConversationId { get; set; } 
        public EnumChatMessageType MessageType { get; set; } = EnumChatMessageType.Text;
        public EnumSenderType SenderType { get; set; } = EnumSenderType.User;

        [Required]
        [MaxLength(1000)]
        public string? Content { get; set; } = null!;

        [MaxLength(100)]
        public string? Intent { get; set; }
        public string? Entitiesjson { get; set; }
        public string? RasaResponse { get; set; }
            public virtual Conversation Conversation { get; set; } = null!;
            public virtual ICollection<ChatAttachment> Attachments { get; set; } 
            = new List<ChatAttachment>();

    }}