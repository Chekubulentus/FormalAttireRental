using RentalAttireBackend.Application.Common.Interfaces;

namespace RentalAttireBackend.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor
            )
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;

                return int.TryParse(userIdClaim, out var userId) ? userId : 0;
            }
        }
    }
}
