using EcommerceBackend.Models;
using EcommerceBackend.DTOs;

namespace EcommerceBackend.Services;
public interface IOrderService
{
    Task<Order> CheckoutAsync(int userId, CheckoutRequest request);
    Task<List<Order>> GetUserOrderHistoryAsync(int userId);
    Task<Order> GetOrderDetailsAsync(int userId, int orderId);
}
