using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.DTOs;
public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(100, ErrorMessage = "Email must have maximum of 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [MinLength(8, ErrorMessage = "New password must be at least 8 characters long.")]
    public string NewPassword { get; set; } = string.Empty;
}
