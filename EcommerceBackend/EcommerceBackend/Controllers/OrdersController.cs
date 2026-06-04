using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceBackend.Services;
using EcommerceBackend.Utils;
using EcommerceBackend.DTOs;

namespace EcommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // Checkout endpoint
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var completedOrder = await _orderService.CheckoutAsync(userId, request);

        var responseData = new
        {
            OrderId = completedOrder.Id,
            TotalBill = completedOrder.TotalAmount,
            Status = completedOrder.Status,
        };
        return Ok(ApiResponse<object>.SuccessResponse("Order placed successfully.", responseData));
    }

    // Order History Enpoint
    [HttpGet("history")]
    public async Task<IActionResult> GetOrderHistory()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var orders = await _orderService.GetUserOrderHistoryAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse("Your Order History", orders));
    }
}
