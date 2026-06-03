using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.DTOs;
public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = string.Empty;
}
