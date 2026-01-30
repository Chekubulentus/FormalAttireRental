namespace RentalAttireBackend.Application.Common.Models
{
    public class JwtSettings
    {
        public string Audience { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
}
