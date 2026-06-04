using EcommerceBackend.Models;
namespace EcommerceBackend.Repositories;
public interface ICartRepository
{
    Task<List<CartItem>> GetCartByUserIdAsync(int userId);
    Task<CartItem?> GetCartItemAsync(int userId, int productId);
    Task AddItemAsync(CartItem item);
    Task UpdateItemAsync(CartItem item);
    Task RemoveItemAsync(CartItem item);
    Task ClearCartAsync(int userId);
}
