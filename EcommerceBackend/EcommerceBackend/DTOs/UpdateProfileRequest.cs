using EcommerceBackend.Models;
namespace EcommerceBackend.DTOs;
public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public GenderType? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
}