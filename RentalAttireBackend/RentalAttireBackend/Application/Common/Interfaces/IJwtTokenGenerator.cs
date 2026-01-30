using RentalAttireBackend.Domain.Entities;
using System.Security.Claims;

namespace RentalAttireBackend.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        public string GenerateAccessToken(User user);
        public string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
