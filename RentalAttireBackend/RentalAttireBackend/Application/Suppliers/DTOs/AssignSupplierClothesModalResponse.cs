using RentalAttireBackend.Application.Clothes.DTOs;

namespace RentalAttireBackend.Application.Suppliers.DTOs
{
    public class AssignSupplierClothesModalResponse
    {
        public List<ClotheDTO> Clothes { get; set; } = new();
        public int TotalCount { get; set; }
        public int AssignedClothesCount { get; set; }
        public int UnassignedClothesCount { get; set; }
        public int CurrentPage { get; set; } 
        public int ItemsPerPage { get; set; }
    }
}
