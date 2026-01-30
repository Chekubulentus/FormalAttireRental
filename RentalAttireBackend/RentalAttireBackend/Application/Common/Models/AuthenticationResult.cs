using RentalAttireBackend.Application.Users.DTO;

namespace RentalAttireBackend.Application.Common.Models
{
    public class AuthenticationResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDTO User { get; set; } = null!;
    }
}
