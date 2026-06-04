using EcommerceBackend.Data;
using EcommerceBackend.DTOs;
using EcommerceBackend.Models;
using EcommerceBackend.Repositories;
using EcommerceBackend.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBackend.Services;
public class InventoryService : IInventoryService
{
    private readonly IProductRepository _productRepository;
    private readonly AppDbContext _context;

    public InventoryService(IProductRepository productRepository, AppDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllActiveProductsAsync()
    {
        var rawProducts = await _productRepository.GetAllActiveAsync();
        return rawProducts.Where(p => p.IsActive && p.SellerProfile != null && p.SellerProfile.IsVerfied);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null || !product.IsActive || (product.SellerProfile != null && !product.SellerProfile.IsVerfied))
        {
            return null;
        }
        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsBySellerUserIdAsync(int userId)
    {
        var sellerProfile = await _context.SellerProfiles.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sellerProfile == null) throw new Exception("Seller profile not found.");
        return await _productRepository.GetBySellerIdAsync(sellerProfile.Id);
    }

    public async Task<Product> AddProductAsync(CreateProductRequest request, int userId)
    {
        var sellerProfile = await _context.SellerProfiles.FirstOrDefaultAsync(s => s.UserId == userId);

        if (sellerProfile == null)
        {
            throw new NotFoundException("Seller account record missing in system.");
        }

        var newProduct = new Product
        {
            ProductName = request.ProductName,
            Description = request.Description ?? string.Empty, 
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            SellerProfileId = sellerProfile.Id,
            IsActive = true
        };

        return await _productRepository.AddAsync(newProduct);
    }
}
