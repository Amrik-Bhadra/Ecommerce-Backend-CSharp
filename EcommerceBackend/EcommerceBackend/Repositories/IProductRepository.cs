using EcommerceBackend.Models;
namespace EcommerceBackend.Repositories;
public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
    Product Add(Product product);
    Product Update(Product product);
}
