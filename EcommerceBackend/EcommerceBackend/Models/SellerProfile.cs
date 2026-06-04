using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceBackend.Models;
public class SellerProfile
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [JsonIgnore]
    public User? User { get; set; }

    [Required, MaxLength(200)]
    public string ShopName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string ShopDescription { get; set; } = string.Empty;

    [Column(TypeName = "decimal(3,2)")]
    public decimal SellerRating { get; set; } = 0.00m;

    public bool IsVerfied { get; set; } = false;

    public DateTime DateOfJoining { get; set; }

    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public List<Product> Products { get; set; } = new();
}
