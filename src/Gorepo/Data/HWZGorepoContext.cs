using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Gorepo
{
    public class HWZGorepoContext : DbContext
    {
        public HWZGorepoContext([NotNull] DbContextOptions<HWZGorepoContext> options)
            : base(options)
        {
        }

        public DbSet<HWZWeChatMessage> WeChatMessages { get; set; } = null!;
        public DbSet<HWZWeChatOrder> WeChatOrders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // HWZWeChatMessage
            modelBuilder.Entity<HWZWeChatMessage>()
                .ToTable("WeChatMessage");

            modelBuilder.Entity<HWZWeChatMessage>()
                .HasIndex(m => m.Id)
                .IsUnique();

            modelBuilder.Entity<HWZWeChatMessage>()
                .HasIndex(m => m.MessageId)
                .IsUnique();

            modelBuilder.Entity<HWZWeChatMessage>()
                .HasIndex(m => m.MessageCreateTime);

            modelBuilder.Entity<HWZWeChatMessage>()
                .HasIndex(m => m.OrderId)
                .IsUnique();


            // HWZWeChatOrder
            modelBuilder.Entity<HWZWeChatOrder>()
                .ToTable("WeChatOrder");

            modelBuilder.Entity<HWZWeChatOrder>()
                .HasIndex(m => m.Id)
                .IsUnique();

            modelBuilder.Entity<HWZWeChatOrder>()
                .HasIndex(m => m.OrderId)
                .IsUnique();
        }
    }

    public class HWZModelBase
    {
        public int Id { get; set; }
        public long CreateTime { get; set; }
        public long UpdateTime { get; set; }
    }

    public class HWZWeChatMessage : HWZModelBase
    {
        public int MessageCreateTime { get; set; }
        public string MessageId { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
        public int MessagePublishTime { get; set; }
        public string OrderId { get; set; } = null!;
        public decimal OrderAmount { get; set; }
    }

    public class HWZWeChatOrder : HWZModelBase
    {
        public string OrderId { get; set; } = null!;
        public decimal OrderAmount { get; set; }
        public string OrderCode { get; set; } = null!;
        public bool IsOrderPay { get; set; }
    }
}
