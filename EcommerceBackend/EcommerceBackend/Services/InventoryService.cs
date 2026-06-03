using EcommerceBackend.DTOs;
using EcommerceBackend.Models;
using EcommerceBackend.Repositories;
using EcommerceBackend.Services;
namespace EcommerceBackend.Services;
public class InventoryService : IInventoryService
{
    private readonly IProductRepository _productRepository;

    public InventoryService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IEnumerable<Product> GetAllProducts()
    {
        return _productRepository.GetAll();
    }
    public Product? GetProductById(int id)
    {
        return _productRepository.GetById(id);
    }
    public Product AddProduct(CreateProductRequest request)
    {
        var newProduct = new Product
        {
            ProductName = request.ProductName,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
        };

        return _productRepository.Add(newProduct);
    }
}
