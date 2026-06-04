using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EcommerceBackend.DTOs;
using EcommerceBackend.Services;
using EcommerceBackend.Utils;

namespace EcommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // Add item to cart
    // POST: api/cart/add-to-cart
    [HttpPost("add-to-cart")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
        return Ok(ApiResponse<object>.SuccessResponse("Product added to cart successfully!"));
    }

    // Update item quantity in cart
    // PUT: api/cart/update-quantity
    [HttpPut("update-quantity")]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _cartService.UpdateQuantityAsync(userId, request.ProductId, request.Action);
        return Ok(ApiResponse<object>.SuccessResponse("Quantity Updated Successfully!"));
    }

    // Remove item from cart
    // DELETE: api/cart/remove-from-cart/{productId}
    [HttpDelete("remove-from-cart/{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _cartService.RemoveFromCartAsync(userId, productId);
        return Ok(ApiResponse<object>.SuccessResponse("Product removed from cart successfully!"));
    }

    // Get cart summary
    // GET: api/cart/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetCartSummary()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var cartSummary = await _cartService.GetUserCartSummaryAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse("Cart Summary Fetched Successfully!", cartSummary));
    }

    // Clear cart
    // DELETE: api/cart/clear-cart
    [HttpDelete("clear-cart")]
    public async Task<IActionResult> ClearCart()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _cartService.ClearCartAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse("Cart cleared successfully!"));
    }
}
