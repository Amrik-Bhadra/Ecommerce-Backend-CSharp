using System.ComponentModel.DataAnnotations; // for validation attributes

namespace EcommerceBackend.DTOs;
public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required!")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name should have atleast 3 characters")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product description is required!")]
    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 100000.00, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Range(0, 1000, ErrorMessage = "Stock must be greater than or equal to 0.")]
    public int StockQuantity { get; set; }
}
