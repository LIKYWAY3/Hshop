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
            base.OnModelCreating(builder);

<<<<<<< HEAD
            builder.Entity<Payment>()
            .HasIndex(p => p.OrderId)
            .IsUnique();
            builder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
=======
            // =========================
            // INDEX
            // =========================
>>>>>>> 7f17eb6ad10110ae4636743f4696003d0c12e04f

            builder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            builder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            builder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique();

            builder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique();
<<<<<<< HEAD
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

=======

            builder.Entity<Payment>()
                .HasIndex(p => p.OrderId)
                .IsUnique();

            // =========================
            // CATEGORY SELF RELATIONSHIP
            // =========================

            builder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // PRODUCT - CATEGORY
            // =========================

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // PRODUCT IMAGE - PRODUCT
            // =========================

            builder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // CART - USER
            // =========================

            builder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // CART ITEM - CART
            // =========================

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // CART ITEM - PRODUCT
            // =========================

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // ORDER - USER
            // =========================

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // ORDER - COUPON
            // =========================

            builder.Entity<Order>()
                .HasOne(o => o.Coupon)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CouponId)
                .OnDelete(DeleteBehavior.SetNull);

            // =========================
            // ORDER ITEM - ORDER
            // =========================

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // ORDER ITEM - PRODUCT
            // =========================

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // ORDER - PAYMENT ONE TO ONE
            // =========================

            builder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // REVIEW - PRODUCT
            // =========================

            builder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // REVIEW - USER
            // =========================

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
>>>>>>> 7f17eb6ad10110ae4636743f4696003d0c12e04f
    }
}