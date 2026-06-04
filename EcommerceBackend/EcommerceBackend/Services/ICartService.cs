using EcommerceBackend.DTOs;
namespace EcommerceBackend.Services;
public interface ICartService
{
    Task AddToCartAsync(int userId, int productId, int quantity);
    Task RemoveFromCartAsync(int userId, int productId);
    Task UpdateQuantityAsync(int userId, int productId, UpdateAction action);
    Task<object> GetUserCartSummaryAsync(int userId);
    Task ClearCartAsync(int userId);
}
