using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Data;
using EcommerceBackend.Models;

namespace EcommerceBackend.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public void CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public Order? GetOrderById(int orderId)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == orderId);
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            // latest order on top
            return _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
    }
}
