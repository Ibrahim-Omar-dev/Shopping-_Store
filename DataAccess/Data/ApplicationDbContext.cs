using DataModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class ApplicationDbContext :IdentityDbContext<IdentityUser>
    {
        public DbSet<Product> products { get; set; }
        public DbSet<ImageProduct> imageProducts{ get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<Company> companies{ get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingCart>()
    .Property(e => e.Id)
    .ValueGeneratedOnAdd();
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
