using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Rentals.Queries.FilterRentals
{
    public class FilterRentalsQueryHandler : IRequestHandler<FilterRentalsQuery, Result<RentalPageResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IRentalRepository _rentalRepo;

        public FilterRentalsQueryHandler(
            IMapper mapper,
            IRentalRepository rentalRepo
            )
        {
            _mapper = mapper;
            _rentalRepo = rentalRepo;
        }

        public async Task<Result<RentalPageResponse>> Handle(FilterRentalsQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<RentalPageResponse>.FailureWithErrorType("Invalid request.", ErrorType.BadRequest);

            var rentals = await _rentalRepo.FilterRentalItemsAsync(
                request.Status,
                request.SearchQuery,
                request.StartingDate,
                request.EndingDate,
                request.CurrentPage,
                request.ItemsPerPage,
                cancellationToken
                );

            var totalRevenue = await _rentalRepo.GetAllRentalsTotalRevenue(cancellationToken);
            var analytics = await _rentalRepo.GetRentalAnalyticsAsync(cancellationToken);

            if (rentals.Items.Count() == 0)
                return Result<RentalPageResponse>.FailureWithErrorType("No rentals currently registered.", ErrorType.NotFound);


            var rentalsDto = _mapper.Map<PagedResult<RentalDTO>>(rentals);

            return Result<RentalPageResponse>.Success(new RentalPageResponse
            {
                Items = rentalsDto.Items,
                CurrentPage = request.CurrentPage,
                ItemsPerPage = request.ItemsPerPage,
                TotalCount = rentals.TotalCount,
                TotalRevenue = totalRevenue,
                StatusCounts = analytics.StatusCounts,
                OverdueCount = analytics.OverdueCount,
                DueSoonCout = analytics.DueSoonCount
            });
        }
    }
}