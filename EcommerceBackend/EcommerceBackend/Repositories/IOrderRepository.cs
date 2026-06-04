using EcommerceBackend.Models;

namespace EcommerceBackend.Repositories;
public interface IOrderRepository
{
    Task CreateOrderAsync(Order order);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<List<Order>> GetOrdersByUserIdAsync(int userId);
}
