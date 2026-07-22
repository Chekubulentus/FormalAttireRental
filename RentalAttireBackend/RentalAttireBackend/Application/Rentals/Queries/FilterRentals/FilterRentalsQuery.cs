using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;

namespace RentalAttireBackend.Application.Rentals.Queries.FilterRentals
{
    public class FilterRentalsQuery : IRequest<Result<RentalPageResponse>>
    {
        public string? CategoryType { get; set; } = string.Empty;
        public string? SearchQuery { get; set; } = string.Empty;
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }
    }
}
