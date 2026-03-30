using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public int PersonId { get; set; }
        public bool IsGoogleAccount { get; set; } = false;

        //NavProp
        [JsonIgnore]
        public Person Person { get; set; } = null!;
        [JsonIgnore]
        public Customer? Customer { get; set; }
        [JsonIgnore]
        public Employee? Employee { get; set; }
    }
}
