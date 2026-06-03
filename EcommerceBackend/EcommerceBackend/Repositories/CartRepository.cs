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
    public List<CartItem> GetCartByUserId(int userId)
    {
        return _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToList();
    }

    public CartItem? GetCartItem(int userId, int productId)
    {
        return _context.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
    }

    public void AddItem(CartItem item)
    {
        _context.CartItems.Add(item);
        _context.SaveChanges();
    }

    public void UpdateItem(CartItem item)
    {
        _context.CartItems.Update(item);
        _context.SaveChanges();
    }
    public void RemoveItem(CartItem item)
    {
        _context.CartItems.Remove(item);
        _context.SaveChanges();
    }
    public void ClearCart(int userId)
    {
        var items = _context.CartItems.Where(c => c.UserId == userId).ToList();
        if (items.Any())
        {
            _context.CartItems.RemoveRange(items);
            _context.SaveChanges();
        }
    }
}
