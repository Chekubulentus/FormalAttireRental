using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; }

        //NavProp
        [JsonIgnore]
        public Employee? Employee { get; set; } 
        [JsonIgnore]
        public Customer? Customer { get; set; } 
    }
}
