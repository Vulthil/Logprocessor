using LogProcessor.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LogProcessor.Infrastructure
{
    public class LogContext : DbContext
    {
        public LogContext(DbContextOptions<LogContext> opts) : base(opts)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LogMessageEntity>().OwnsOne(p => p.Message, navBuilder =>
            {
                navBuilder.HasIndex(p => p.SessionId);
            });
        }

        public DbSet<ViolatingMessage> ViolatingMessages { get; set; }
        public DbSet<PoisonedMessage> PoisonedMessages { get; set; }
        public DbSet<LogMessageEntity> LogMessages { get; set; }
        public DbSet<SessionType> SessionTypes { get; set; }


    }
}