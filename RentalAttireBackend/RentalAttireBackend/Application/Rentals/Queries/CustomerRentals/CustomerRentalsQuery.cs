using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;

namespace RentalAttireBackend.Application.Rentals.Queries.CustomerRentals
{
    public class CustomerRentalsQuery : IRequest<Result<PagedResult<RentalDTO>>>
    {
        public string? SearchQuery { get; set; } = string.Empty;
        public string? RentalStatus { get; set; } = string.Empty;
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
