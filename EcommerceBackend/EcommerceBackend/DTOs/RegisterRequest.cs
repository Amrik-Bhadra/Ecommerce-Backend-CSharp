using EcommerceBackend.Models;
using System.ComponentModel.DataAnnotations;
namespace EcommerceBackend.DTOs;
public class RegisterRequest
{
    [Required, MaxLength(100, ErrorMessage = "Username can have maximum 100 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(100, ErrorMessage = "Email must have maximum of 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8, ErrorMessage = "Password must be of minimum 8 characters.")]
    public String Password { get; set;  } = string.Empty;
    public UserRole Role { get; set; }

    [MaxLength(200, ErrorMessage = "Shop name cannot exceed 200 characters.")]
    public string? ShopName { get; set; }

    [MaxLength(500, ErrorMessage = "Shop description cannot exceed 500 characters.")]
    public string? ShopDescription { get; set; }
}
