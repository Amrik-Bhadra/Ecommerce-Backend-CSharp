using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using EcommerceBackend.Repositories;
using EcommerceBackend.Models;
using EcommerceBackend.DTOs;
namespace EcommerceBackend.Services;
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    public User Register(RegisterRequest request)
    {
        // check if email already exists or not
        if(_userRepo.GetByEmail(request.Email) != null)
        {
            throw new Exception("Email already exists!");
        }

        var newUser = new User
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
        };

        return _userRepo.Add(newUser);
    }

    public string Login(LoginRequest request)
    {
        // search for user using email
        var user = _userRepo.GetByEmail(request.Email);
        if(user == null)
        {
            throw new Exception("User not found!");
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new Exception("Invalid Credentials!");
        }

        return GeneratedJwtToken(user);
    }

    private string GeneratedJwtToken(User user) 
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7), // 7 days validity
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
