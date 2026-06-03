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
}
