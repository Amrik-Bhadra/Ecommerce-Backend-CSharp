using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace EcommerceBackend.Utils;
public static class ClaimExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new Exception("User ID claim not found");
        }
        return int.Parse(userIdClaim);
    }
}
