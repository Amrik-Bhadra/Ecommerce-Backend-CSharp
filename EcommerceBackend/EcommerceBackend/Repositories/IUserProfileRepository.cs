using EcommerceBackend.Models;

namespace EcommerceBackend.Repositories;
public interface IUserProfileRepository
{
    Task<CustomerProfile?> GetProfileByUserIdAsync(int userId);
    Task<CustomerProfile> UpdateProfileAsync(CustomerProfile profile);
    Task<List<Address>> GetAddressesByProfileIdAsync(int profileId);
    Task<Address> AddAddressAsync(Address address);
    Task<Address?> GetAddressByIdAsync(int addressId);
    Task<Address> UpdateAddressAsync(Address address);
    Task DeleteAddressAsync(Address address);
}
