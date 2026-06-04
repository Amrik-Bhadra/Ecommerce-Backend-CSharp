using EcommerceBackend.Models;
using EcommerceBackend.Repositories;
using EcommerceBackend.DTOs;
using EcommerceBackend.Exceptions;

namespace EcommerceBackend.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _profileRepo;

    public UserProfileService(IUserProfileRepository profileRepo)
    {
        _profileRepo = profileRepo;
    }

    public async Task<CustomerProfile> GetProfileAsync(int userId)
    {
        var profile = await _profileRepo.GetProfileByUserIdAsync(userId);
        if (profile == null) throw new NotFoundException("Customer profile not found.");
        return profile;
    }

    public async Task<CustomerProfile> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var profile = await _profileRepo.GetProfileByUserIdAsync(userId);
        if (profile == null) throw new NotFoundException("Customer profile not found.");

        profile.FullName = request.FullName;
        profile.PhoneNumber = request.PhoneNumber;
        profile.Gender = request.Gender;
        profile.DateOfBirth = request.DateOfBirth;

        return await _profileRepo.UpdateProfileAsync(profile);
    }

    public async Task<List<Address>> GetMyAddressesAsync(int userId)
    {
        var profile = await _profileRepo.GetProfileByUserIdAsync(userId);
        if (profile == null) throw new NotFoundException("Customer profile not found.");

        return await _profileRepo.GetAddressesByProfileIdAsync(profile.Id);
    }

    public async Task<Address> AddAddressAsync(int userId, AddAddressRequest request)
    {
        var profile = await _profileRepo.GetProfileByUserIdAsync(userId);
        if (profile == null) throw new NotFoundException("Customer profile not found.");

        var newAddress = new Address
        {
            CustomerProfileId = profile.Id,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2 ?? string.Empty,
            City = request.City,
            State = request.State,
            Pincode = request.Pincode,
            AddressType = request.AddressType
        };

        return await _profileRepo.AddAddressAsync(newAddress);
    }

    public async Task<Address> UpdateAddressAsync(int userId, int addressId, AddAddressRequest request)
    {
        var profile = await _profileRepo.GetProfileByUserIdAsync(userId);
        if (profile == null) throw new NotFoundException("Customer profile not found.");

        var address = await _profileRepo.GetAddressByIdAsync(addressId);
        if (address == null) throw new NotFoundException("Address not found.");

        if (address.CustomerProfileId != profile.Id)
        {
            throw new BadRequestException("Unauthorized to modify this address!");
        }

        // Step 3: Data update karo
        address.AddressLine1 = request.AddressLine1;
        address.AddressLine2 = request.AddressLine2 ?? string.Empty;
        address.City = request.City;
        address.State = request.State;
        address.Pincode = request.Pincode;
        address.AddressType = request.AddressType;

        return await _profileRepo.UpdateAddressAsync(address);
    }

    public async Task DeleteAddressAsync(int userId, int addressId)
    {
        var profile = await _profileRepo.GetProfileByUserIdAsync(userId);
        if (profile == null) throw new Exception("Customer profile not found.");

        var address = await _profileRepo.GetAddressByIdAsync(addressId);
        if (address == null) throw new Exception("Address not found.");

        // Validation check for cross-user attack
        if (address.CustomerProfileId != profile.Id)
        {
            throw new BadRequestException("Unauthorized to delete this address!");
        }

        await _profileRepo.DeleteAddressAsync(address);
    }
}