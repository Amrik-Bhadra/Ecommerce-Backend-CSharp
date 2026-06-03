using EcommerceBackend.Models;
namespace EcommerceBackend.Repositories;
public interface IUserRepository
{
    User? GetByEmail(string email);
    User? GetByUsername(string username);
    User Add(User user);
}
