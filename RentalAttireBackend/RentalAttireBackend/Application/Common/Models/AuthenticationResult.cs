using RentalAttireBackend.Application.Users.DTO;

namespace RentalAttireBackend.Application.Common.Models
{
    public class AuthenticationResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsProfileComplete { get; set; } = false;
    }
}
