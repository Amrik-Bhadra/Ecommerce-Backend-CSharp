using EcommerceBackend.Models;
namespace EcommerceBackend.Repositories;
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> AddAsync(User user, string? shopName, string? shopDescription);
    Task<User> UpdateAsync(User user);
}
