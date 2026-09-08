using MediatR;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Suppliers.DTOs;

namespace RentalAttireBackend.Application.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommand : IRequest<Result<bool>>
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<int> AssignClotheIds { get; set; } = new();
        public List<int> UnassignClotheIds { get; set; } = new();
    }
}
