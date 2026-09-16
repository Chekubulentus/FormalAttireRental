using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Suppliers.DTOs;

namespace RentalAttireBackend.Application.Suppliers.Queries.AssignClothesModal
{
    public class AssignClothesModalQuery : IRequest<Result<AssignSupplierClothesModalResponse>>
    {
        public int? SupplierId { get; set; }
        public string? SearchQuery { get; set; } = string.Empty;
        public string? Category { get; set; } = string.Empty;
        public string? Gender { get; set; } = "Others";
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
