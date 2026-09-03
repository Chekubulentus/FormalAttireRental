using RentalAttireBackend.Application.Employees.Queries.GetEmployeeById;
using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<Clothe> ClothesAvailable { get; set; } = new();
        public int EmployeeId { get; set; } 

        // Nav Prop
        [JsonIgnore]
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        [JsonIgnore]
        public Employee Employee { get; set; } = null!;
    }
}
