using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Gorepo.Data
{
    public class HWZContext : DbContext
    {
        public HWZContext([NotNull] DbContextOptions<HWZContext> options)
            : base(options)
        {
        }

        public DbSet<HWZMessage> Messages { get; set; } = null!;
        public DbSet<HWZOrder> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // HWZMessage
            modelBuilder.Entity<HWZMessage>()
                .HasIndex(m => m.Id)
                .IsUnique();

            modelBuilder.Entity<HWZMessage>()
                .HasIndex(m => m.MessageServerId)
                .IsUnique();

            modelBuilder.Entity<HWZMessage>()
                .HasIndex(m => m.MessageCreateTime);

            modelBuilder.Entity<HWZMessage>()
                .HasIndex(m => m.OrderId)
                .IsUnique();


            // HWZOrder
            modelBuilder.Entity<HWZOrder>()
                .HasIndex(m => m.Id)
                .IsUnique();

            modelBuilder.Entity<HWZOrder>()
                .HasIndex(m => m.OrderId)
                .IsUnique();
        }
    }

    public class HWZMessage
    {
        public int Id { get; set; }
        public int MessageCreateTime { get; set; }
        public string MessageServerId { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
        public int MessagePublishTime { get; set; }
        public string OrderId { get; set; } = null!;
        public decimal OrderAmount { get; set; }
        public long CreateTime { get; set; }
        public long UpdateTime { get; set; }
    }

    public class HWZOrder
    {
        public int Id { get; set; }
        public string OrderId { get; set; } = null!;
        public decimal OrderAmount { get; set; }
        public string Code { get; set; } = null!;
        public bool IsPay { get; set; }
        public long CreateTime { get; set; }
        public long UpdateTime { get; set; }

        public HWZMessage? Message { get; set; }
    }
}
