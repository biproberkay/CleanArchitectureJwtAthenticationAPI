using Application.Settings;
using Shared.Communicate.Identity.Token;
using Shared.Wrapper;
using System.Security.Claims;

namespace Application.Interfaces.Services.Identity
{
    public interface ITokenService : IService
    {
        string GenerateJwt(IEnumerable<Claim> claims, JwtSettings jwtSettings);// jwt for login-register so on...
    }
}
