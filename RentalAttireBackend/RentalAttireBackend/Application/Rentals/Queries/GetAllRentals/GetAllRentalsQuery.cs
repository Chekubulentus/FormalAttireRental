using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;

namespace RentalAttireBackend.Application.Rentals.Queries.GetAllRentals
{
    public class GetAllRentalsQuery : IRequest<Result<List<RentalDTO>>>
    {
    }
}
