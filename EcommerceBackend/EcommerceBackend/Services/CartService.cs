using EcommerceBackend.Models;
using EcommerceBackend.Repositories;
using EcommerceBackend.DTOs;
using EcommerceBackend.Exceptions;

namespace EcommerceBackend.Services;
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;
    public CartService(ICartRepository cartRepo, IProductRepository productRepo)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
    }

    // Add item to cart
    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null || !product.IsActive) throw new NotFoundException("Product is not available.");
        if (product.StockQuantity < quantity) throw new BadRequestException("Not enough stock available.");

        var existingCartItem = await _cartRepo.GetCartItemAsync(userId, productId);
        if (existingCartItem != null)
        {
            int totalNewQty = existingCartItem.Quantity + quantity;
            if (product.StockQuantity < totalNewQty) throw new BadRequestException("Not enough stock available.");

            existingCartItem.Quantity = totalNewQty;
            await _cartRepo.UpdateItemAsync(existingCartItem);
        }
        else
        {
            var newItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow
            };
            await _cartRepo.AddItemAsync(newItem);
        }
    }

    public async Task UpdateQuantityAsync(int userId, int productId, UpdateAction action)
    {
        var existingItem = await _cartRepo.GetCartItemAsync(userId, productId);
        if (existingItem == null) throw new BadRequestException("Item not found in cart.");

        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null || !product.IsActive) throw new BadRequestException("Product is no longer available.");

        switch (action)
        {
            case UpdateAction.Increment:
                if (product.StockQuantity < existingItem.Quantity + 1)
                    throw new BadRequestException($"Only {product.StockQuantity} items left.");
                existingItem.Quantity += 1;
                await _cartRepo.UpdateItemAsync(existingItem);
                break;

            case UpdateAction.Decrement:
                if (existingItem.Quantity - 1 <= 0) await _cartRepo.RemoveItemAsync(existingItem);
                else
                {
                    existingItem.Quantity -= 1;
                    await _cartRepo.UpdateItemAsync(existingItem);
                }
                break;
        }
    }

    public async Task RemoveFromCartAsync(int userId, int productId)
    {
        var existingItem = await _cartRepo.GetCartItemAsync(userId, productId);
        if (existingItem != null) await _cartRepo.RemoveItemAsync(existingItem);
    }

    public async Task ClearCartAsync(int userId) => await _cartRepo.ClearCartAsync(userId);

    public async Task<object> GetUserCartSummaryAsync(int userId)
    {
        var cartItems = await _cartRepo.GetCartByUserIdAsync(userId);
        decimal grandTotal = cartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity);

        return new
        {
            UserId = userId,
            TotalItemsCount = cartItems.Count,
            GrandTotal = grandTotal,
            Items = cartItems.Select(item => new
            {
                ProductId = item.ProductId,
                ProductName = item.Product?.ProductName ?? "Unknown",
                UnitPrice = item.Product?.Price ?? 0,
                Quantity = item.Quantity,
                TotalPrice = (item.Product?.Price ?? 0) * item.Quantity
            }).ToList()
        };
    }
}
