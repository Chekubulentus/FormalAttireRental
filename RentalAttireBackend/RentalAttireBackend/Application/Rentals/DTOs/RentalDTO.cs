using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Rentals.DTOs
{
    public class RentalDTO
    {
        public string RentalCode { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int TotalAmount { get; set; }
        public int DepositAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
        public List<RentalItemDTO> RentalItems { get; set; } = new();
    }
}
