using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Clothe : BaseEntity
    {
        public string ClotheCode { get; set; } = string.Empty;
        public string ClotheName { get; set; } = string.Empty;
        public int CategoryId { get; set; } // Suit, Barong, Gown, Tuxedo
        public string Color { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty; 
        public string Size { get; set; } = string.Empty; // S, M, L, XL, XXL
        public ClotheGender Gender { get; set; }  
        public int StockQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int RentalPrice { get; set; }
        public int DepositAmount { get; set; }
        public int RentalDurationDays { get; set; }
        public Condition Condition { get; set; }
        public int ReservedQuantity { get; set; }
        public string ProfileImagePath { get; set; } = string.Empty;
        public bool IsAvailable => AvailableQuantity > 0;
        public int RentalCount { get; set; }
        public double UnitCost { get; set; }
        public int SupplierId { get; set; }

        //NavProp
        [JsonIgnore]
        public Category Category { get; set; } = null!;
        [JsonIgnore]
        public List<RentalItem> RentalItems { get; set; } = new();
        [JsonIgnore]
        public Supplier Supplier { get; set; } = null!;
        [JsonIgnore]
        public List<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new();
    }
    public enum ClotheGender    
    {
        Male = 0,
        Female = 1,
        Unisex = 2
    }
    public enum Condition
    {
        Good = 0,
        Returned = 1,
        Damaged = 2,
        Cleaning = 3
    }
}
