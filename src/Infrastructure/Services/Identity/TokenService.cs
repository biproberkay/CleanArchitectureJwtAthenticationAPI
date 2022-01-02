using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Services.Identity;
using Application.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Identity
{
    public class TokenService : ITokenService
    {
        public string GenerateJwt(IEnumerable<Claim> claims, JwtSettings settings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var issuer = string.Empty;
            var audience = string.Empty;
            var notBefore = DateTime.UtcNow.AddSeconds(10);
            var expires = DateTime.UtcNow.AddDays(2);
            var credentials = GetSigningCredentials(settings.Secret);

            var jwtToken = new JwtSecurityToken(issuer, audience, claims, notBefore, expires, credentials);

            var token = tokenHandler.WriteToken(jwtToken); // compact serialization format of jwt 
            return token;
        }
        private SigningCredentials GetSigningCredentials(string jwtSecret)
        {
            var secret = Encoding.UTF8.GetBytes(jwtSecret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }
    }
}
