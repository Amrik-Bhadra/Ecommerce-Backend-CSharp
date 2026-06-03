using System.ComponentModel.DataAnnotations;
namespace EcommerceBackend.DTOs;
public class RegisterRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8, ErrorMessage = "Password must be of minimum 8 characters.")]
    public String Password { get; set;  } = string.Empty;
}
