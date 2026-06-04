using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using EcommerceBackend.Repositories;
using EcommerceBackend.Models;
using EcommerceBackend.DTOs;
using EcommerceBackend.Exceptions;

namespace EcommerceBackend.Services;
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository userRepo, IConfiguration config, IEmailService emailService)
    {
        _userRepo = userRepo;
        _config = config;
        _emailService = emailService;
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        // check if email already exists or not
        if(await _userRepo.GetByEmailAsync(request.Email) != null)
        {
            throw new Exception("Email already exists!");
        }
        if (request.Role == UserRole.Seller && string.IsNullOrWhiteSpace(request.ShopName))
        {
            throw new Exception("Shop Name is mandatory for Seller registration.");
        }

        var newUser = new User
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            SecurityStamp = Guid.NewGuid().ToString(), // safety stamp initialization
            // Admin role cannot be directly registered
            Role = request.Role == UserRole.Admin ? UserRole.Customer : request.Role,
            CreatedAt = DateTime.UtcNow,
        };

        return await _userRepo.AddAsync(newUser, request.ShopName, request.ShopDescription);
    }

    public async Task<string> LoginAsync(LoginRequest request)
    {
        // search for user using email
        var user = await _userRepo.GetByEmailAsync(request.Email);
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
            new Claim(ClaimTypes.Email, user.Email),
            // packing security stamp inside token
            new Claim("SecurityStamp", user.SecurityStamp),
            new Claim(ClaimTypes.Role, user.Role.ToString())
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

    // Forgot password
    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _userRepo.GetByEmailAsync(email);
        if (user == null)
        {
            throw new NotFoundException("User not found!");
        }
        
        var random = new Random();
        string generatedOtp = random.Next(100000, 999999).ToString(); // Generate a 6-digit OTP

        user.ResetOtp = generatedOtp;
        user.OtpExpiry = DateTime.UtcNow.AddMinutes(5); // OTP valid for 5 minutes
        user.IsOtpVerified = false;

        // update the user data in database
        await _userRepo.UpdateAsync(user);

        // send the email with the generated OTP
        await _emailService.SendOtpEmailAsync(email, generatedOtp);
    }

    public async Task<bool> VerifyOtpAsync(string email, string otp)
    {
        var user = await _userRepo.GetByEmailAsync(email);
        if(user == null) throw new NotFoundException("User not found!");

        // check if OTP is valid
        // check for otp expiry
        if(user.ResetOtp != otp || user.OtpExpiry < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid or expired OTP!");
        }

        user.IsOtpVerified = true;
        await _userRepo.UpdateAsync(user);
        return true;
    }

    // Reset password
    public async Task ResetPasswordAsync(string email, string newPassword)
    {
        var user = await _userRepo.GetByEmailAsync(email);
        if(user == null) throw new NotFoundException("User not found!");

        // if attacker directly tries to hit this endpoint without verifying OTP, then also we will not allow them to reset the password
        if (!user.IsOtpVerified)
        {
            throw new BadRequestException("OTP not verified!");
        }

        // encrypt the new password and store it in database
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

        // new stamp, so old tokens become invalid
        user.SecurityStamp = Guid.NewGuid().ToString();

        // Security cleanup: Clear OTP and related fields after successful password reset
        user.ResetOtp = null;
        user.OtpExpiry = null;
        user.IsOtpVerified = false;

        await _userRepo.UpdateAsync(user);
    }
}
