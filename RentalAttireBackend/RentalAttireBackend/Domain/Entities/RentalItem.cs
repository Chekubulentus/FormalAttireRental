using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class RentalItem
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public int ClotheId { get; set; }
        public int RentalPrice { get; set; }
        public int Quantity { get; set; }
        public int DepositAmount { get; set; }
        public int TotalAmount => RentalPrice * Quantity;
        //NavProp  
        public Rental Rental { get; set; } = null!;
        [JsonIgnore]
        public Clothe Clothe { get; set; } = null!;
    }
}
