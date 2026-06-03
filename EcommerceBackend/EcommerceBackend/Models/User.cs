using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EcommerceBackend.Models;
public class User
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty; // never store password as plain text, store as hash
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(6, ErrorMessage = "OTP must be 6 characters long.")]
    [Column(TypeName ="char(6)")]
    public string? ResetOtp { get; set; }
    public DateTime? OtpExpiry { get; set; }
    public bool IsOtpVerified { get; set; } = false;
}
