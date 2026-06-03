using Microsoft.AspNetCore.Mvc;
using EcommerceBackend.DTOs;
using EcommerceBackend.Services;

namespace EcommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var createdUser = _authService.Register(request);
        var responseData = new
        {
            Id = createdUser.Id,
            Username = createdUser.Username,
            Email = createdUser.Email,
        };
        return Ok(ApiResponse<object>.SuccessResponse("User registered successfully!", responseData));
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var token = _authService.Login(request);
        var tokenData = new { Token = token };
        return Ok(ApiResponse<object>.SuccessResponse("User logged in successfully!", tokenData));
    }
}
