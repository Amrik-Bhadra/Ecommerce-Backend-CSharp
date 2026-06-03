using EcommerceBackend.DTOs;
using EcommerceBackend.Models;
namespace EcommerceBackend.Services;
public interface IInventoryService
{
    IEnumerable<Product> GetAllProducts();
    Product? GetProductById(int id);

    Product AddProduct(CreateProductRequest request);
}
