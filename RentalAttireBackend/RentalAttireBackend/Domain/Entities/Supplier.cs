using RentalAttireBackend.Application.Employees.Queries.GetEmployeeById;
using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty; // Required
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty; // Required
        public List<Clothe> ClothesAvailable { get; set; } = new();
        public int EmployeeId { get; set; }
        public int AssignedClothesCount => ClothesAvailable.Count();

        // Nav Prop
        [JsonIgnore]
        public List<PurchaseOrder> PurchaseOrders { get; set; } = new();
        [JsonIgnore]
        public Employee Employee { get; set; } = null!;
    }
}
