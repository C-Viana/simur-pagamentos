using System.Security.Claims;

namespace simur_backend.Auth.Contract
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GerPrincipalFromExpiredToken(string token);
    }
}
