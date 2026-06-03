using EcommerceBackend.Models;
namespace EcommerceBackend.Repositories;
public interface ICartRepository
{
    List<CartItem> GetCartByUserId(int userId);
    CartItem? GetCartItem(int userId, int productId);
    void AddItem(CartItem item);
    void UpdateItem(CartItem item);
    void RemoveItem(CartItem item);
    void ClearCart(int userId);
}
