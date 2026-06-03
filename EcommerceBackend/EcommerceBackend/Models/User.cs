using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EcommerceBackend.Models;
public class User
{
    public int Id { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(100, ErrorMessage = "Email can have maximum 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(100, ErrorMessage = "Username can have maximum 100 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(8, ErrorMessage = "Password must be of minimum 8 characters.")]
    public string PasswordHash { get; set; } = string.Empty; // never store password as plain text, store as hash
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // for tracking force logout
    [Required]
    [MaxLength(50)]
    public string SecurityStamp { get; set;  } = Guid.NewGuid().ToString();

    [StringLength(6, ErrorMessage = "OTP must be 6 characters long.")]
    [Column(TypeName ="char(6)")]
    public string? ResetOtp { get; set; }
    public DateTime? OtpExpiry { get; set; }
    public bool IsOtpVerified { get; set; } = false;
}
