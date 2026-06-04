using EcommerceBackend.Models;
namespace EcommerceBackend.Repositories;
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllActiveAsync();
    Task<IEnumerable<Product>> GetBySellerIdAsync(int sellerProfileId);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
}
