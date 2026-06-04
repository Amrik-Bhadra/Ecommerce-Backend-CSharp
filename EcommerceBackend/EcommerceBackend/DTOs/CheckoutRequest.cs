using System.ComponentModel.DataAnnotations;
namespace EcommerceBackend.DTOs;
public class CheckoutRequest
{
    [Required(ErrorMessage = "Shipping address must be required.")]
    public int AddressId { get; set; }
}