using EcommerceBackend.Data;
using EcommerceBackend.Models;
using EcommerceBackend.Repositories;
using EcommerceBackend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBackend.Services;
public class OrderService : IOrderService
{
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly AppDbContext _context;  // required for running the transactions

    public OrderService(
        ICartRepository cartRepo,
        IProductRepository productRepo,
        IOrderRepository orderRepo,
        AppDbContext context)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
        _orderRepo = orderRepo;
        _context = context;
    }

    public async Task<Order> CheckoutAsync(int userId, CheckoutRequest request)
    {
        var customerProfile = await _context.CustomerProfiles.FirstOrDefaultAsync(cp => cp.UserId == userId);
        if (customerProfile == null) throw new Exception("Customer profile not registered.");

        var targetAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.AddressId);
        if (targetAddress == null) throw new Exception("Selected Address does not exist.");

        // Hacker prevention test: Is this address owned by this customer profile?
        if (targetAddress.CustomerProfileId != customerProfile.Id)
        {
            throw new Exception("Security Alert: Unauthorized shipping address token bypass attempted!");
        }

        var cartItems = await _cartRepo.GetCartByUserIdAsync(userId);
        if (cartItems == null || !cartItems.Any()) throw new Exception("Cart is empty");

        decimal grandTotal = cartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity);

        // Async transactional context execution
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var newOrder = new Order
            {
                UserId = userId,
                AddressId = request.AddressId, // 🎯 Enforced into table constraint safely
                OrderDate = DateTime.UtcNow,
                TotalAmount = grandTotal,
                Status = "Pending",
                OrderItems = new List<OrderItem>()
            };

            foreach (var cartItem in cartItems)
            {
                var product = await _productRepo.GetByIdAsync(cartItem.ProductId);
                if (product == null || !product.IsActive)
                    throw new Exception($"Product {cartItem.Product?.ProductName} is unavailable.");

                if (product.StockQuantity < cartItem.Quantity)
                    throw new Exception($"Product {product.ProductName} has insufficient stock.");

                product.StockQuantity -= cartItem.Quantity;
                await _productRepo.UpdateAsync(product);

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price
                };
                newOrder.OrderItems.Add(orderItem);
            }

            await _orderRepo.CreateOrderAsync(newOrder);
            await _cartRepo.ClearCartAsync(userId);
            await transaction.CommitAsync();

            return newOrder;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Checkout transactional engine failed: " + ex.Message);
        }
    }

    public async Task<List<Order>> GetUserOrderHistoryAsync(int userId) => await _orderRepo.GetOrdersByUserIdAsync(userId);
}
