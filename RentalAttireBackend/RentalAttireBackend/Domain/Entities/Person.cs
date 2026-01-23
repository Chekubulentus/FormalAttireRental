using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Person : BaseEntity
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public Gender Age { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Barangay { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        //NavProp
        [JsonIgnore]
        public Customer Customer { get; set; } = null!;
        [JsonIgnore]
        public Employee Employee { get; set; } = null!;
    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
        Others = 2
    }

    public enum MaritalStatus
    {
        Single = 0,
        Married = 1,
        Seperated = 2,
        Divorced = 3,
        Widowed = 4,
        Annulled = 5
    }
}
