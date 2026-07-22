using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Rentals.Queries.GetAllRentals
{
    public class GetAllRentalsQueryHandler : IRequestHandler<GetAllRentalsQuery, Result<List<RentalDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRentalRepository _rentalRepo;
        public GetAllRentalsQueryHandler(
            IMapper mapper,
            IRentalRepository rentalRepo
            )
        {
            _mapper = mapper;
            _rentalRepo = rentalRepo;
        }
        public async Task<Result<List<RentalDTO>>> Handle(GetAllRentalsQuery query, CancellationToken cancellationToken)
        {
            var rentals = await _rentalRepo.GetAllRentalsAsync(cancellationToken);

            if (rentals.Count == 0)
                return Result<List<RentalDTO>>.Failure("No rentals currently registered.");

            var rentalsDto = _mapper.Map<List<RentalDTO>>(rentals);

            return Result<List<RentalDTO>>.Success(rentalsDto);
        }
    }
}
