using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Role : BaseEntity
    {
        public RolePosition RolePosition { get; set; }

        //NavProp
        [JsonIgnore]
        public List<Employee> Employees { get; set; } = new();
    }

    public enum RolePosition
    {
        Administrator = 0,
        ClothesManager= 1,
        Cashier = 2
    }
}
