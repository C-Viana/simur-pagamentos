using Microsoft.IdentityModel.Tokens;
using simur_backend.Auth.Config;
using simur_backend.Auth.Contract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace simur_backend.Auth
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly TokenConfiguration _configs;

        public TokenGenerator(TokenConfiguration configs)
        {
            _configs = configs;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            byte[] secretKey = Encoding.UTF8.GetBytes(_configs.Secret);

            SigningCredentials signinCredentials = new (new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);
            
            var tokenOptions = new JwtSecurityToken(
                issuer: _configs.Issuer,
                audience: _configs.Audience,
                claims: claims,
                expires: DateTimeOffset.Now.AddMinutes(_configs.TokenLifetimeMinutes).DateTime,
                signingCredentials: signinCredentials
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GerPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configs.Secret))
            };
            JwtSecurityTokenHandler tokenHandler = new();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}
