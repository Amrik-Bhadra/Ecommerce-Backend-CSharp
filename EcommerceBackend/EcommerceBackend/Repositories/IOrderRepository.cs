using EcommerceBackend.Models;

namespace EcommerceBackend.Repositories;
public interface IOrderRepository
{
    void CreateOrder(Order order);
    Order? GetOrderById(int orderId);
    List<Order> GetOrdersByUserId(int userId);
}
