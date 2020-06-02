
using Microsoft.EntityFrameworkCore;
using NetworkMonitor.Objects;

namespace NetworkMonitor.Data
{
    public class MonitorContext : DbContext
    {
      
        public MonitorContext(DbContextOptions<MonitorContext> options) : base(options)
        {
        }

        public DbSet<MonitorPingInfo> MonitorPingInfos{ get; set; }
        public DbSet<PingInfo> PingInfos { get; set; }

      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MonitorPingInfo>().ToTable("MonitorPingInfo");
            modelBuilder.Entity<PingInfo>().ToTable("PingInfo");
           
        }


    }
}
