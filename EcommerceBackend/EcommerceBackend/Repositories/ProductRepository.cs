using EcommerceBackend.Data;
using EcommerceBackend.Models;
using EcommerceBackend.Repositories;

namespace EcommerceBackend.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Product> GetAll()
    {
        return _context.Products.ToList();
    }

    public Product? GetById(int id)
    {
        return _context.Products.Find(id);
    }

    public Product Add(Product product) 
    { 
        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }

    public Product Update(Product product)
    {
        _context.Products.Update(product);
        return product;
    }
}
