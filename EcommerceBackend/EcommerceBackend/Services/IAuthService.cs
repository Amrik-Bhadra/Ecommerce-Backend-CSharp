using EcommerceBackend.DTOs;
using EcommerceBackend.Models;
namespace EcommerceBackend.Services;
public interface IAuthService
{
    Task<User> RegisterAsync(RegisterRequest request);
    Task<string> LoginAsync(LoginRequest request);
    Task ForgotPasswordAsync(string email);
    Task<bool> VerifyOtpAsync(string email, string otp);
    Task ResetPasswordAsync(string email, string newPassword);
}
