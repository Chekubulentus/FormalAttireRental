using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Suppliers.DTOs;

namespace RentalAttireBackend.Application.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommand : IRequest<Result<bool>>
    {
        public string SupplierName { get; set; } = string.Empty; //Required
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty; // Required
        public List<int> ClotheIds { get; set; } = new();
    }
}
