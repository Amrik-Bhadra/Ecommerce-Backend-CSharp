using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EcommerceBackend.Services;
using EcommerceBackend.DTOs;

namespace EcommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _profileService;

    public UserProfileController(IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    // PROFILE ROUTES
    [HttpGet("my-profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var profile = await _profileService.GetProfileAsync(userId);
        //return Ok(new { message = "Profile fetched successfully!", data = profile });
        return Ok(ApiResponse<object>.SuccessResponse(message: "Profile fetched successfully!", data: profile));
    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedProfile = await _profileService.UpdateProfileAsync(userId, request);
        return Ok(ApiResponse<object>.SuccessResponse(message: "Profile updated successfully!", data: updatedProfile));
    }

    // ADDRESS ROUTES
    [HttpGet("my-addresses")]
    public async Task<IActionResult> GetAddresses()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var addresses = await _profileService.GetMyAddressesAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse(message: "Addresses fetched successfully!", data: addresses));
    }

    [HttpPost("add-address")]
    public async Task<IActionResult> AddAddress([FromBody] AddAddressRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var newAddress = await _profileService.AddAddressAsync(userId, request);
        return Ok(ApiResponse<object>.SuccessResponse(message: "Address added successfully!", data: newAddress));
    }

    [HttpPut("update-address/{id}")]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddAddressRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var updatedAddress = await _profileService.UpdateAddressAsync(userId, id, request);
            return Ok(ApiResponse<object>.SuccessResponse(message: "Address updated successfully!", data: updatedAddress));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("delete-address/{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            await _profileService.DeleteAddressAsync(userId, id);
            return Ok(ApiResponse<object>.SuccessResponse(message: "Address deleted successfully!"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}