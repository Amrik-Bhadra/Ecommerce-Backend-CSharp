using Microsoft.AspNetCore.Mvc;
using EcommerceBackend.DTOs;
using EcommerceBackend.Services;
using Microsoft.AspNetCore.Authorization;

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
        var cookieOptins = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("X-Auth-Token", token, cookieOptins);
        return Ok(ApiResponse<object>.SuccessResponse(message: "User logged in successfully!"));
    }

    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        _authService.ForgotPassword(request.Email);
        return Ok(ApiResponse<string>.SuccessResponse("OTP sent to your email!"));
    }

    [HttpPost("verify-otp")]
    public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        bool isValid = _authService.VerifyOtp(request.Email, request.Otp);
        if (isValid)
        {
            return Ok(ApiResponse<string>.SuccessResponse("OTP verified successfully!"));
        }
        else
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("Invalid OTP!"));
        }
    }

    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
    {
        _authService.ResetPassword(request.Email, request.NewPassword);
        return Ok(ApiResponse<string>.SuccessResponse("Password reset successfully!"));
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("X-Auth-Token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });

        return Ok(ApiResponse<string>.SuccessResponse("User logged out successfully! Please delete the token on client side."));
    }
}
