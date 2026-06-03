using System.ComponentModel.DataAnnotations;
namespace EcommerceBackend.DTOs;
public enum UpdateAction
{
    Increment, // 0
    Decrement, // 1
}
public class UpdateQuantityRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public UpdateAction Action { get; set; }
}