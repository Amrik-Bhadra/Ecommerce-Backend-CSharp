using System.ComponentModel.DataAnnotations; // for string length
using System.ComponentModel.DataAnnotations.Schema;  // For column type\
using System.Text.Json.Serialization;
namespace EcommerceBackend.Models;
public class Product
{
    // .NET automatically makes this primary key and Identity (Auto-Increment)
    public int Id { get; set; }

    // In database it will be nvarchar(100)
    [Required]
    [MaxLength(100)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int LikesCount { get; set; } = 0;

    [Column(TypeName = "decimal(3,2)")]
    public decimal AverageRating { get; set; } = 0.00m;

    public int TotalReviewsCount { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    [Required]
    public int SellerProfileId { get; set; }

    [ForeignKey("SellerProfileId")]
    public SellerProfile? SellerProfile { get; set; }
}
