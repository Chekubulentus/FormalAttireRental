using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;

namespace RentalAttireBackend.Application.Rentals.Commands.RentalTransaction
{
    public class RentalTransactionCommand : IRequest<Result<bool>>
    {
        public DateTime PickupDate { get; set; } 
        public DateTime ReturnDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; //Gcash, Cash
        public string GcashRefNum { get; set; } = string.Empty;
        public string GcashRefName { get; set; } = string.Empty;
        public List<RentalItemRequest> RentalItems { get; set; } = new();
    }
}
