using Microsoft.EntityFrameworkCore.Design.Internal;
using RentalAttireBackend.Application.Clothes.DTOs;

namespace RentalAttireBackend.Application.Rentals.DTOs
{
    public class RentalItemDTO
    {
        public int Id { get; set; }
        public string RentalCode { get; set; } = string.Empty;
        public ClotheDTO Clothe { get; set; } = new();
        public int RentalPrice { get; set; }
        public int Quantity { get; set; }
        public int TotalAmount => Clothe.RentalPrice * Quantity;
    }
}
