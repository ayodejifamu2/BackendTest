using Microsoft.EntityFrameworkCore;
using Trading.Domain;

namespace Trading.Infrastructure.Data
{
    public class TradingDbContext(DbContextOptions<TradingDbContext> options) : DbContext(options)
    {
        public DbSet<Trade> Trades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Trade>(entity =>
            {
                entity.HasKey(t => t.TradeId);
                entity.Property(t => t.Asset).IsRequired();
                entity.Property(t => t.Quantity).IsRequired();
                entity.Property(t => t.Price).IsRequired();
                entity.Property(t => t.TradeType).IsRequired();
                entity.Property(t => t.Timestamp).IsRequired();
                entity.Property(t => t.Status).IsRequired();
                entity.Property(t => t.FailureReason);
            });
        }
    }
} 