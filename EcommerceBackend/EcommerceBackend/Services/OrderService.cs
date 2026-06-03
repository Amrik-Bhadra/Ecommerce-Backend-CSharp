using EcommerceBackend.Data;
using EcommerceBackend.Models;
using EcommerceBackend.Repositories;

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

    public Order Checkout(int userId)
    {
        // fetch user's cart where products are there
        var cartItems = _cartRepo.GetCartByUserId(userId);
        if (cartItems == null || !cartItems.Any())
            throw new Exception("Cart is empty");

        // calculate grand total of cart : LINQ
        decimal grandTotal = cartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity);

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            // create a new order with the cart details and grand total
            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = grandTotal,
                Status = "Pending",
                OrderItems = new List<OrderItem>()
            };

            // run loop on cart items and validate inventory and deduct inventory
            foreach(var cartItem in cartItems)
            {
                var product = _productRepo.GetById(cartItem.ProductId);
                if (product == null)
                    throw new Exception($"Product {cartItem.Product?.ProductName} is currently not available.");

                // Edge case: if stock becomes insufficient during checkout
                if (product.StockQuantity < cartItem.Quantity)
                    throw new Exception($"Product {product.ProductName} has insufficient stock.");

                // realtime inventory deduction
                product.StockQuantity -= cartItem.Quantity;
                _productRepo.Update(product);


                // OrderItem list entry added
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price
                };
                newOrder.OrderItems.Add(orderItem);
            }

            // save the order to the database
            _orderRepo.CreateOrder(newOrder);

            _context.SaveChanges();

            // Empty the cart when order gets placed successfully
            _cartRepo.ClearCart(userId);

            transaction.Commit();

            return newOrder;
        }
        catch (Exception ex)
        {
            // Rollback the transaction in case of any failure during checkout
            // deducted stock will be reverted and order will not be created
            transaction.Rollback();
            if (ex.InnerException != null)
            {
                throw new Exception($"SQL ERROR: {ex.InnerException.Message}");
            }
            throw new Exception("Checkout failed: " + ex.Message);
        }
    }

    public List<Order> GetUserOrderHistory(int userId)
    {
        return _orderRepo.GetOrdersByUserId(userId);
    }
}
