using EcommerceBackend.DTOs;
using EcommerceBackend.Exceptions;
using EcommerceBackend.Models;
using EcommerceBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace EcommerceBackend.Controllers;

// tells the framework that it is a Web API controller
[ApiController]
// the route when hit, this controller method to be called
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    // Constructor Injection
    // .NET will automatically pass the object of IInventoryService in this in background
    public ProductsController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _inventoryService.GetAllActiveProductsAsync();
        return Ok(ApiResponse<object>.SuccessResponse(data: products));
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _inventoryService.GetProductByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException("Product not found or currently unavailable.");
        }

        return Ok(ApiResponse<object>.SuccessResponse(data: product));
    }

    // GET: api/products/my-shop
    [HttpGet("my-shop")]
    [Authorize(Roles = "Seller")] // Only seller can access their own factory dashboard
    public async Task<IActionResult> GetMyShopProducts()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var products = await _inventoryService.GetProductsBySellerUserIdAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse(data: products, message: "Shop inventory loaded successfully."));
    }

    // POST: api/products
    [HttpPost]
    [Authorize(Roles = "Seller")] // Admin can monitor, but only Seller creates product for their shop
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        // Token extract context identity safely
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var newProduct = await _inventoryService.AddProductAsync(request, userId);
            var response = ApiResponse<object>.SuccessResponse(data: newProduct, message: "Product created successfully");

            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}
