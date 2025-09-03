using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Ihjezly.Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        var userId =
            principal?.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            principal?.FindFirstValue(ClaimTypes.NameIdentifier); // fallback

        return Guid.TryParse(userId, out var parsed)
            ? parsed
            : throw new ApplicationException("User id is unavailable");
    }


    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
               throw new ApplicationException("User identity is unavailable");
    }
}