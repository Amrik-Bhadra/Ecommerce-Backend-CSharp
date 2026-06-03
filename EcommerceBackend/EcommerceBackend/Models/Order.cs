using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceBackend.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Shipped, Delivered, Cancelled

    public List<OrderItem> OrderItems { get; set; } = new();
}
