using Microsoft.IdentityModel.Tokens;
using simur_backend.Models.Entities;
using simur_backend.Models.Entities.Payments;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace simur_backend.Utilities
{
    public class AlgorithmJwt
    {
        public static string EncodePaymentToJWT(PixPayload pixPayloadData)
        {
            JwtSecurityTokenHandler handler = new();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("txid", pixPayloadData.Txid),
                new Claim("amount", pixPayloadData.Amount.ToString()),
                new Claim("currency", pixPayloadData.Currency),
                new Claim("merchant_document", pixPayloadData.MerchantDocument)
            };

            var token = new JwtSecurityToken(
                issuer: "pix.simurpagamentos.com.br",
                audience: "pix",
                claims: claims,
                notBefore: DateTime.Now,
                expires: pixPayloadData.ExpiresAt,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
