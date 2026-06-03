using EcommerceBackend.Models;

namespace EcommerceBackend.Services;
public interface IOrderService
{
    Order Checkout(int userId);
    List<Order> GetUserOrderHistory(int orderId);
}
