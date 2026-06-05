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
        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.SellerProfile)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p!.SellerProfile)

                .Include(o => o.ShippingAddress) 
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
