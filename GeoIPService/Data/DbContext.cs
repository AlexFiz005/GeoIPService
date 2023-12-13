using GeoIPService.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoIPService.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
             : base(options)
        {
        }

        public DbSet<GeoIPInfo> GeoIPInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeoIPInfo>().HasKey(g => g.Id);
        }

    }
}
