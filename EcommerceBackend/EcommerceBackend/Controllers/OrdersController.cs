using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceBackend.Services;
using EcommerceBackend.Utils;
using EcommerceBackend.DTOs;

namespace EcommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // Checkout endpoint
    [HttpPost("checkout")]
    public IActionResult Checkout()
    {
        int userId = User.GetUserId();
        var completedOrder = _orderService.Checkout(userId);
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
    public IActionResult GetOrderHistory()
    {
        int userId = User.GetUserId();
        var orders = _orderService.GetUserOrderHistory(userId);
        return Ok(ApiResponse<object>.SuccessResponse("Your Order History", orders));
    }
}
