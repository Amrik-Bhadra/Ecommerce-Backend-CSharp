using EcommerceBackend.DTOs;
using EcommerceBackend.Models;
namespace EcommerceBackend.Services;
public interface IAuthService
{
    User Register(RegisterRequest request);
    string Login(LoginRequest request);
    void ForgotPassword(string email);
    bool VerifyOtp(string email, string otp);
    void ResetPassword(string email, string newPassword);
}
