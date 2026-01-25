using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string CustomerCode { get; set; } = string.Empty;
        public int TotalRentals { get; set; }
        public double TotalSpent { get; set; }
        public int PersonId { get; set; }

        //NavProp
        [JsonIgnore]
        public Person? Person { get; set; } 
    }
}
