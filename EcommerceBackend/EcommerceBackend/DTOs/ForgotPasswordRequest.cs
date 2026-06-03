using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.DTOs;
public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(100, ErrorMessage = "Email must have maximum of 100 characters")]
    public string Email { get; set; } = string.Empty;
}
