using RentalAttireBackend.Application.Clothes.DTOs;

namespace RentalAttireBackend.Application.Suppliers.DTOs
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<ClotheDTO> ClothesAvailable { get; set; } = new();
        public string CreatedByEmployee { get; set; } = string.Empty;
    }
}
