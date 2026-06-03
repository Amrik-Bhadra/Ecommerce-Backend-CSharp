using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceBackend.Models;
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [JsonIgnore]
    [ForeignKey("OrderId")]
    public Order? Order { get; set; }

    [ForeignKey("ProductId")]
    public Product? Product { get; set; }
}
