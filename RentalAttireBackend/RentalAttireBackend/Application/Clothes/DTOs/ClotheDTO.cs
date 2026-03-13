namespace RentalAttireBackend.Application.Clothes.DTOs
{
    public class ClotheDTO
    {
        public int Id { get; set; }
        public string ClotheCode { get; set; } = string.Empty;
        public string ClotheName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string ClotheGender { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int AvailableQuantity { get; set; } 
        public int RentalPrice { get; set; }
        public int DepositAmount { get; set; }
        public int RentalDurationDays { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; } = string.Empty;
    }
}
