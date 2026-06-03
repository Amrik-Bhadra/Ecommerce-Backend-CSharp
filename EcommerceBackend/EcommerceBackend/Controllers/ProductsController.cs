using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EcommerceBackend.DTOs;
using EcommerceBackend.Models;
using EcommerceBackend.Services;
using EcommerceBackend.Exceptions;
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
    public IActionResult GetAllProducts()
    {
        var products = _inventoryService.GetAllProducts();
        return Ok(ApiResponse<object>.SuccessResponse(data: products));
    }

    // GET: api/products/{id}
    [HttpGet("{id:int}")] // parameters and validation set
    public IActionResult GetProductById(int id) // accepting the parameter in function
    {
        var product = _inventoryService.GetProductById(id);
        if(product == null)
        {
            throw new NotFoundException("Product not found");
        }

        return Ok(ApiResponse<object>.SuccessResponse(data: product));
    }

    // POST: api/products
    [HttpPost]
    // Now without valid JWT this controller cannot be accessed
    [Authorize(Roles = "Admin,Seller")]
    public IActionResult CreateProduct([FromBody] CreateProductRequest request)
    {
        var newProduct = _inventoryService.AddProduct(request);
        var response = ApiResponse<object>.SuccessResponse(data: newProduct, message: "Product created successfully");

        // Industry standard: 201 created status for new record creation
        // new product is created and will be available at following url
        // in response headers, for Location key we have url where the new item can be found
        return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, response);
    }
}
