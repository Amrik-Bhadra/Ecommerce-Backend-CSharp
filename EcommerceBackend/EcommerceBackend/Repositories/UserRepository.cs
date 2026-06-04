using EcommerceBackend.Models;
using EcommerceBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBackend.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
    }
    public async Task<User> AddAsync(User user, string? shopName, string? shopDescription)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            if (user.Role == UserRole.Seller)
            {
                var sellerProfile = new SellerProfile
                {
                    UserId = user.Id,
                    ShopName = string.IsNullOrWhiteSpace(shopName) ? $"{user.Username}'s Shop" : shopName,
                    ShopDescription = shopDescription ?? string.Empty,
                    SellerRating = 0.00m,
                    IsVerfied = false,
                    DateOfJoining = DateTime.UtcNow
                };
                await _context.SellerProfiles.AddAsync(sellerProfile);
            }
            else if (user.Role == UserRole.Customer)
            {
                var customerProfile = new CustomerProfile
                {
                    UserId = user.Id,
                    FullName = user.Username,
                    PhoneNumber = string.Empty,
                    Gender = null,
                    DateOfBirth = null
                };
                await _context.CustomerProfiles.AddAsync(customerProfile);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return user;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
