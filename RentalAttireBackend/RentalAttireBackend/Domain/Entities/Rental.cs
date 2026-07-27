using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Rental : BaseEntity
    {
        public string RentalCode { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public DateTime RentalDate { get; set; } // Also pick-up date
        public DateTime ReturnDate { get; set; }
        public int TotalAmount { get; set; }
        public int DepositAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
         
        //NavProp
        [JsonIgnore]
        public List<RentalItem> RentalItems { get; set; } = new();
        [JsonIgnore]
        public Customer Customer { get; set; } = null!;
    }

    public enum PaymentMethod
    {
        Cash = 0,
        Gcash = 1
    }
}
