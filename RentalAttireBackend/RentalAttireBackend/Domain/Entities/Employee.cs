using RentalAttireBackend.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public double Salary { get; set; }
        public int RoleId { get; set; }
        public int PersonId { get; set; }

        //NavProp
        [JsonIgnore]
        public Role? Role { get; set; } 
        [JsonIgnore]
        public Person? Person { get; set; }
        [JsonIgnore]
        public List<User>? Users { get; set; }
    }
}
