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

    // Forgot password
    public void ForgotPassword(string email)
    {
        var user = _userRepo.GetByEmail(email);
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
        _userRepo.Update(user);

        // send the email with the generated OTP
        _emailService.SendOtpEmail(email, generatedOtp);
    }

    public bool VerifyOtp(string email, string otp)
    {
        var user = _userRepo.GetByEmail(email);
        if(user == null) throw new NotFoundException("User not found!");

        // check if OTP is valid
        // check for otp expiry
        if(user.ResetOtp != otp || user.OtpExpiry < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid or expired OTP!");
        }

        user.IsOtpVerified = true;
        _userRepo.Update(user);
        return true;
    }

    // Reset password
    public void ResetPassword(string email, string newPassword)
    {
        var user = _userRepo.GetByEmail(email);
        if(user == null) throw new NotFoundException("User not found!");

        // if attacker directly tries to hit this endpoint without verifying OTP, then also we will not allow them to reset the password
        if (!user.IsOtpVerified)
        {
            throw new BadRequestException("OTP not verified!");
        }

        // encrypt the new password and store it in database
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

        // Security cleanup: Clear OTP and related fields after successful password reset
        user.ResetOtp = null;
        user.OtpExpiry = null;
        user.IsOtpVerified = false;

        _userRepo.Update(user);
    }
}
