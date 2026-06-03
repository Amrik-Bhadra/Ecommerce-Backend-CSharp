using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EcommerceBackend.DTOs;
using EcommerceBackend.Services;
using EcommerceBackend.Utils;

namespace EcommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    public IActionResult AddToCart([FromBody] AddToCartRequest request)
    {
        int userId = User.GetUserId();
        _cartService.AddToCart(userId, request.ProductId, request.Quantity);
        return Ok(ApiResponse<object>.SuccessResponse("Product added to cart successfully!"));
    }

    // Update item quantity in cart
    // PUT: api/cart/update-quantity
    [HttpPut("update-quantity")]
    public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest request)
    {
        int userId = User.GetUserId();
        _cartService.UpdateQuantity(userId, request.ProductId, request.Action);
        return Ok(ApiResponse<object>.SuccessResponse("Quantity Updated Successfully!"));
    }

    // Remove item from cart
    // DELETE: api/cart/remove-from-cart/{productId}
    [HttpDelete("remove-from-cart/{productId}")]
    public IActionResult RemoveFromCart(int productId)
    {
        int userId = User.GetUserId();
        _cartService.RemoveFromCart(userId, productId);
        return Ok(new { Message = "Product removed from cart successfully!" });
    }

    // Get cart summary
    // GET: api/cart/summary
    [HttpGet("summary")]
    public IActionResult GetCartSummary()
    {
        int userId = User.GetUserId();
        var cartSummary = _cartService.GetUserCartSummary(userId);
        return Ok(ApiResponse<object>.SuccessResponse("Cart Summary Fetched Successfully!", cartSummary));
    }

    // Clear cart
    // DELETE: api/cart/clear-cart
    [HttpDelete("clear-cart")]
    public IActionResult ClearCart()
    {
        int userId = User.GetUserId();
        _cartService.ClearCart(userId);
        return Ok(ApiResponse<object>.SuccessResponse("Cart cleared successfully!"));
    }
}
