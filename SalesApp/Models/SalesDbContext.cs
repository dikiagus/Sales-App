using Microsoft.EntityFrameworkCore;

namespace SalesApp.Models
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options)
            : base(options) { }

        public DbSet<COM_CUSTOMER> COM_CUSTOMER { get; set; }
        public DbSet<SO_ORDER> SO_ORDER { get; set; }
        public DbSet<SO_ITEM> SO_ITEM { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SO_ORDER>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.SO_ORDERS)
                .HasForeignKey(o => o.COM_CUSTOMER_ID);

            modelBuilder.Entity<SO_ITEM>()
                .HasOne(i => i.Order)
                .WithMany(o => o.SO_ITEMS)
                .HasForeignKey(i => i.SO_ORDER_ID);
        }
    }
}
