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
    }

    public class HWZMessage
    {
        public int Id { get; set; }
        public int CreateTime { get; set; }
        public string ServerId { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int PublishTime { get; set; }
        public string OrderId { get; set; } = null!;
        public decimal OrderAmount { get; set; }
    }
}
