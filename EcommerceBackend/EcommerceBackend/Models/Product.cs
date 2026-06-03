using System.ComponentModel.DataAnnotations; // for string length
using System.ComponentModel.DataAnnotations.Schema;  // For column type
namespace EcommerceBackend.Models;
public class Product
{
    // .NET automatically makes this primary key and Identity (Auto-Increment)
    public int Id { get; set; }

    // In database it will be nvarchar(100)
    [Required]
    [MaxLength(100)]
    public string ProductName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }
}
