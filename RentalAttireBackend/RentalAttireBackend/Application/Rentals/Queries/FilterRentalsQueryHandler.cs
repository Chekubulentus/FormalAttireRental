using AutoMapper;
using Google.Apis.Upload;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Rentals.Queries
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
                return Result<RentalPageResponse>.Failure("Invalid request.");

            if (request.CurrentPage == 0 || request.ItemsPerPage == 0)
                return Result<RentalPageResponse>.Failure("Current & Items Per Page is invalid.");
            try
            {
                var rentals = await _rentalRepo.FilterRentalItemsAsync(
                    request.CategoryType,
                    request.SearchQuery,
                    request.StartingDate,
                    request.EndingDate,
                    request.CurrentPage,
                    request.ItemsPerPage,
                    cancellationToken
                    );

                if (rentals.TotalCount == 0)
                    return Result<RentalPageResponse>.Failure("No rentals currently registered.");

                var totalRevenue = rentals.Items.SelectMany(r => r.RentalItems)
                    .Sum(ri => ri.TotalAmount);

                var totalCount = rentals.TotalCount;

                var rentalsDto = _mapper.Map<List<RentalDTO>>(rentals);

                return Result<RentalPageResponse>.Success(new RentalPageResponse
                {
                    Items = rentalsDto,
                    CurrentPage = request.CurrentPage,
                    ItemsPerPage = request.ItemsPerPage,
                    TotalCount = totalCount,
                    TotalRevenue = totalRevenue
                });
            }catch(Exception e)
            {
                return Result<RentalPageResponse>.Failure(e.Message);
            }
        }
    }
}
