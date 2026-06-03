using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace EcommerceBackend.DTOs;
public class LoginRequest
{
    [Required, EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(100, ErrorMessage = "Emnail can have maximum of 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8, ErrorMessage = "Password must be of minimum 8 characters.")]
    public string Password { get; set; } = string.Empty;
}
