using System.ComponentModel.DataAnnotations;
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
}
