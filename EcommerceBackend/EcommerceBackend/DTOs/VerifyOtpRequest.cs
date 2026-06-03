using System.ComponentModel.DataAnnotations;

namespace EcommerceBackend.DTOs;
public class VerifyOtpRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(100, ErrorMessage = "Email must have maximum of 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP is required.")]
    [StringLength(6, ErrorMessage = "OTP must be of 6 digits.")]
    public string Otp { get; set; } = string.Empty;
}
