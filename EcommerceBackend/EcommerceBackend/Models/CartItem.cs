using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.Models;
public class CartItem
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required, Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
    public int Quantity { get; set; }
    
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    // using this EF Core automatically join Product data
    [ForeignKey("ProductId")]
    public Product? Product { get; set; }
}
