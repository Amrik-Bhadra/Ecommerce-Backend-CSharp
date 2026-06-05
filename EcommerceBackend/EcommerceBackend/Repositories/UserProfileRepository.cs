using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Data;
using EcommerceBackend.Models;

namespace EcommerceBackend.Repositories;
public class UserProfileRepository : IUserProfileRepository
{
    private readonly AppDbContext _context;
    public UserProfileRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<CustomerProfile?> GetProfileByUserIdAsync(int userId)
    {
        return await _context.CustomerProfiles
            .Include(cp => cp.Addresses)
            .FirstOrDefaultAsync(cp => cp.UserId == userId);
    }
    public async Task<CustomerProfile> UpdateProfileAsync(CustomerProfile profile)
    {
        _context.CustomerProfiles.Update(profile);
        await _context.SaveChangesAsync();
        return profile;
    }
    public async Task<List<Address>> GetAddressesByProfileIdAsync(int profileId)
    {
        return await _context.Addresses.Where(a => a.CustomerProfileId == profileId).ToListAsync();
    }
    public async Task<Address> AddAddressAsync(Address address)
    {
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        return address;
    }
    public async Task<Address?> GetAddressByIdAsync(int addressId)
    {
        return await _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId);
    }

    public async Task<Address> UpdateAddressAsync(Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
        return address;
    }

    public async Task DeleteAddressAsync(Address address)
    {
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
    }
}
