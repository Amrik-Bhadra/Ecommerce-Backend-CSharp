using EcommerceBackend.Models;
using EcommerceBackend.DTOs;
namespace EcommerceBackend.Services;
public interface IUserProfileService
{
    Task<CustomerProfile> GetProfileAsync(int userId);
    Task<CustomerProfile> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<List<Address>> GetMyAddressesAsync(int userId);
    Task<Address> AddAddressAsync(int userId, AddAddressRequest request);
    Task<Address> UpdateAddressAsync(int userId, int addressId, AddAddressRequest request);
    Task DeleteAddressAsync(int userId, int addressId);
}
