using EcommerceBackend.DTOs;
using EcommerceBackend.Models;
namespace EcommerceBackend.Services;
public interface IInventoryService
{
    Task<IEnumerable<Product>> GetAllActiveProductsAsync();
    Task<IEnumerable<Product>> GetProductsBySellerUserIdAsync(int userId);
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> AddProductAsync(CreateProductRequest request, int userId);
}
