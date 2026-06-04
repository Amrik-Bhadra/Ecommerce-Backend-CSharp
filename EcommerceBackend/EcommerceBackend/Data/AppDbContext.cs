using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Models;
namespace EcommerceBackend.Data;
public class AppDbContext : DbContext
{
    // receives constructor database settings
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // this tells .NET framework that we want to create a 'Products' named table from Product model
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<SellerProfile> SellerProfiles { get; set; }
    public DbSet<CustomerProfile> CustomerProfiles { get; set; }
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Restrict automatic deletion btw SellerProfile and Product
        modelBuilder.Entity<Product>()
            .HasOne(p => p.SellerProfile)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.SellerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict Auto deletion between Order and Address
        modelBuilder.Entity<Order>()
            .HasOne(o => o.ShippingAddress)
            .WithMany()
            .HasForeignKey(o => o.AddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
