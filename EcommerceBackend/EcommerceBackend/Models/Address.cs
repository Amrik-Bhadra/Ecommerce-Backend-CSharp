using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceBackend.Models;
public class Address
{
    public int Id { get; set; }
    [Required]
    public int CustomerProfileId { get; set; }

    [ForeignKey("CustomerProfileId")]
    [JsonIgnore]
    public CustomerProfile? CustomerProfile { get; set; }

    [Required, MaxLength(100)]
    public string AddressLine1 { get; set; } = string.Empty;

    [MaxLength(100)]
    public string AddressLine2 { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string State { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string Pincode { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string AddressType { get; set; } = "Home";
}
