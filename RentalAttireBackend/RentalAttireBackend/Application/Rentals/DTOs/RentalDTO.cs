using RentalAttireBackend.Application.Customers.DTOs;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Rentals.DTOs
{
    public class RentalDTO
    {
        public int Id { get; set; }
        public string RentalCode { get; set; } = string.Empty;
        public CustomerDTO Customer { get; set; } = new();
        public DateTime PickupDate { get; set; } 
        public DateTime ReturnDate { get; set; }
        public int TotalAmount { get; set; }
        public int DepositAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<RentalItemDTO> RentalItems { get; set; } = new();
        public string Status { get; set; } = string.Empty;
    }
}
