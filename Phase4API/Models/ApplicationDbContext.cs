using Microsoft.EntityFrameworkCore;
using Phase4.Models;

namespace Phase4API.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
               : base(options)
        {
        }


        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<Producer> Producer { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Login> Login { get; set; }

        public DbSet<Admins> Admins { get; set; }
        public DbSet<Packages> Packages { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<Cart> Cart { get; set; }


        public DbSet<AdminUserWishList> AdminUserWishList { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Producer)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.ProducerId)
                .OnDelete(DeleteBehavior.Cascade); // Example of configuring cascade delete
        }

    }
}
