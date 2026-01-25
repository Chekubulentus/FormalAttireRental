using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public double Salary { get; set; }
        public int RoleId { get; set; }
        public int PersonId { get; set; }

        //NavProp
        [JsonIgnore]
        public Role? Role { get; set; } 
        [JsonIgnore]
        public Person? Person { get; set; } 
    }
}
