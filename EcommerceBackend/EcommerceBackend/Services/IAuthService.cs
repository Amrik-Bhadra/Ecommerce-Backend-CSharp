using EcommerceBackend.DTOs;
using EcommerceBackend.Models;
namespace EcommerceBackend.Services;
public interface IAuthService
{
    User Register(RegisterRequest request);
    string Login(LoginRequest request);
}
