using ASPtestShop.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatAttachment> ChatAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // BỎ: Cấu hình Index cho UserName và Email vì IdentityDbContext đã tự làm.
            base.OnModelCreating(builder);

            builder.Entity<Payment>()
            .HasIndex(p => p.OrderId)
            .IsUnique();
            builder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .IsUnique();
            builder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();
            builder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique();
            builder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId);
            builder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique();
            // Conversation -> User (1 - 1)
            builder.Entity<Conversation>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Conversation -> ChatMessages (1 - N)
            builder.Entity<Conversation>()
                .HasMany(c => c.ChatMessages)
                .WithOne(cm => cm.Conversation)
                .HasForeignKey(cm => cm.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        
            // ChatMessage -> ChatAttachments (1 - N)
            builder.Entity<ChatMessage>()
                .HasMany(cm => cm.Attachments)
                .WithOne(a => a.ChatMessage)
                .HasForeignKey(a => a.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.Entity<Conversation>()
                .HasIndex(c => c.UserId);

            builder.Entity<ChatMessage>()
                .HasIndex(cm => cm.ConversationId);

            builder.Entity<ChatMessage>()
                .HasIndex(cm => cm.Intent);

            // Enum
            builder.Entity<ChatMessage>()
                .Property(cm => cm.SenderType)
                .HasConversion<int>();

            builder.Entity<ChatMessage>()
                .Property(cm => cm.MessageType)
                .HasConversion<int>();

            builder.Entity<ChatAttachment>()
                .Property(ca => ca.AttachmentType)
                .HasConversion<int>();


        }

    }
}