using EcommerceBackend.Models;
using EcommerceBackend.Data;

namespace EcommerceBackend.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public User? GetByEmail(string email)
    {
        return _context.Users.FirstOrDefault(user => user.Email == email);
    }

    public User? GetByUsername(string username)
    {
        return _context.Users.FirstOrDefault(user => user.Username == username);
    }
    public User Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }
    public User Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
        return user;
    }
}
