using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Role : BaseEntity
    {
        public RolePosition RolePosition { get; set; }

        //NavProp
        [JsonIgnore]
        public List<Employee>? Employees { get; set; } 
    }

    public enum RolePosition
    {
        Administrator = 1,
        ClothesManager= 2,
        Cashier = 3
    }
}
