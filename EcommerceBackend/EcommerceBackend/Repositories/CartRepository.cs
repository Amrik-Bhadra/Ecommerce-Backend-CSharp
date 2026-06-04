using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Data;
using EcommerceBackend.Models;
namespace EcommerceBackend.Repositories;
public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    public CartRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<CartItem>> GetCartByUserIdAsync(int userId)
    {
        return await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<CartItem?> GetCartItemAsync(int userId, int productId)
    {
        return await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
    }

    public async Task AddItemAsync(CartItem item)
    {
        await _context.CartItems.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(CartItem item)
    {
        _context.CartItems.Update(item);
        await _context.SaveChangesAsync();
    }
    public async Task RemoveItemAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }
    public async Task ClearCartAsync(int userId)
    {
        var items = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
        if (items.Any())
        {
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
