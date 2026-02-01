using RentalAttireBackend.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string CustomerCode { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public int TotalRentals { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double TotalSpent { get; set; }  
        public int UserId { get; set; }

        //NavProp
        [JsonIgnore]
        public User? User { get; set; }
    }
}
