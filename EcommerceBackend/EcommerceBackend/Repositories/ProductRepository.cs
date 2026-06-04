using EcommerceBackend.Data;
using EcommerceBackend.Models;
using EcommerceBackend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBackend.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllActiveAsync()
    {
        return await _context.Products
            .Include(p => p.SellerProfile)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetBySellerIdAsync(int sellerProfileId)
    {
        return await _context.Products
            .Where(p => p.SellerProfileId == sellerProfileId)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.SellerProfile)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }
}
