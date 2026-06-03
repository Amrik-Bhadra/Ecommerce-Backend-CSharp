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
    public void AddToCart(int userId, int productId, int quantity)
    {
        var product = _productRepo.GetById(productId);
        if (product == null)
        {
            throw new NotFoundException("Product is not available. Please select another product.");
        }

        // check for stock availability
        if(product.StockQuantity < quantity)
        {
            throw new BadRequestException("Not enough stock available.");
        }

        // check if the item is already in the cart
        var existingCartItem = _cartRepo.GetCartItem(userId, productId);
        if(existingCartItem != null)
        {
            int totalNewQty = existingCartItem.Quantity + quantity;
            if(product.StockQuantity < totalNewQty)
            {
                throw new Exception("Not enough stock available to update the quantity.");
            }
            existingCartItem.Quantity = totalNewQty;
            _cartRepo.UpdateItem(existingCartItem);
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
            _cartRepo.AddItem(newItem);
        }
    }

    // update the item quantity in cart
    public void UpdateQuantity(int userId, int productId, UpdateAction action)
    {
        // check if item already exists in the cart
        var existingItem = _cartRepo.GetCartItem(userId, productId);
        if (existingItem == null) throw new Exception("Item not found in cart.");

        // check if product exists and has enough stock for the update
        var product = _productRepo.GetById(productId);
        if (product == null) throw new Exception("This product is no longer available.");

        switch (action)
        {
            case UpdateAction.Increment:
                int targetQuantity = existingItem.Quantity + 1;
                if (product.StockQuantity < targetQuantity)
                {
                    throw new Exception($"Not enough stock available. Only {product.StockQuantity} items left.");
                }

                existingItem.Quantity = targetQuantity;
                _cartRepo.UpdateItem(existingItem);
                break;

            case UpdateAction.Decrement:
                int lowQuantity = existingItem.Quantity - 1;
                if (lowQuantity <= 0)
                {
                    _cartRepo.RemoveItem(existingItem);
                }
                else
                {
                    existingItem.Quantity = lowQuantity;
                    _cartRepo.UpdateItem(existingItem);
                }
                break;

            default:
                throw new Exception("Invalid update action.");
        }
    }

    // remove item from the cart
    public void RemoveFromCart(int userId, int productId)
    {
        var existingItem = _cartRepo.GetCartItem(userId, productId);
        if (existingItem != null)
        {
            _cartRepo.RemoveItem(existingItem);
        }
    }

    // clear entire cart
    public void ClearCart(int userId)
    {
        _cartRepo.ClearCart(userId);
    }

    // Get cart summary
    public object GetUserCartSummary(int userId)
    {
        var cartItems = _cartRepo.GetCartByUserId(userId);
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
