using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace EcommerceBackend.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GenderType
{
    Male,
    Female,
    Other
}

public class CustomerProfile
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [JsonIgnore]
    public User? User { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(10)]
    [Column(TypeName = "nvarchar(15)")]
    public GenderType? Gender { get; set; } = null;

    public DateTime? DateOfBirth { get; set; }

    public List<Address> Addresses { get; set; } = new();
}
