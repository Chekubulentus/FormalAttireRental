using RentalAttireBackend.Application.Persons.DTO;

namespace RentalAttireBackend.Application.Customers.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public int TotalRentals { get; set; }
        public double TotalSpent { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsGoogleAccount { get; set; }
        public PersonDTO Person { get; set; } = null!;
    }
}
