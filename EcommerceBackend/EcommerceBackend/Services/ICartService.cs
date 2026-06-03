using EcommerceBackend.DTOs;
namespace EcommerceBackend.Services;
public interface ICartService
{
    void AddToCart(int userId, int productId, int quantity);
    void RemoveFromCart(int userId, int productId);
    void UpdateQuantity(int userId, int productId, UpdateAction action);
    object GetUserCartSummary(int userId); // returns entire cart data + total price
    void ClearCart(int userId);
}
