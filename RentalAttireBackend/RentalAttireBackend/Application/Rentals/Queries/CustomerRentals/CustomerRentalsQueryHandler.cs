using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Domain.Interfaces;
using System.Diagnostics.Contracts;

namespace RentalAttireBackend.Application.Rentals.Queries.CustomerRentals
{
    public class CustomerRentalsQueryHandler : IRequestHandler<CustomerRentalsQuery, Result<PagedResult<RentalDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRentalRepository _rentalRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepo;
        private readonly IFileUploadService _fileUploadService;

        public CustomerRentalsQueryHandler(
            IMapper mapper,
            IRentalRepository rentalRepo,
            ICurrentUserService currentUserService,
            IUserRepository userRepo,
            IFileUploadService fileUploadService
            )
        {
            _mapper = mapper;
            _rentalRepo = rentalRepo;
            _currentUserService = currentUserService;
            _userRepo = userRepo;
            _fileUploadService = fileUploadService;
        }

        public async Task<Result<PagedResult<RentalDTO>>> Handle(CustomerRentalsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId == 0)
                return Result<PagedResult<RentalDTO>>.FailureWithErrorType("User does not exist.", ErrorType.NotFound);

            var customer = await _userRepo.GetUserByIdWithCustomerAsync(userId ?? 0, cancellationToken);

            if (customer is null)
                return Result<PagedResult<RentalDTO>>.FailureWithErrorType("Customer could not be found.", ErrorType.NotFound);

            var paginatedRentals = await _rentalRepo.GetCustomerRentalsAsync(
                customer.Customer.Id,
                request.SearchQuery ?? "",
                request.RentalStatus ?? "",
                request.StartingDate,
                request.EndingDate,
                request.CurrentPage,
                request.ItemsPerPage,
                cancellationToken
                );

            if (paginatedRentals.Items.Count() == 0)
                return Result<PagedResult<RentalDTO>>.FailureWithErrorType("No rentals found currently.", ErrorType.NotFound);

            var processedClotheIds = new HashSet<int>();

            foreach(var rentalItem in paginatedRentals.Items.SelectMany(r => r.RentalItems))
            {
                var clothe = rentalItem.Clothe;

                if (clothe is null || clothe.ProfileImagePath is null)
                    continue;

                if (!processedClotheIds.Add(clothe.Id))
                    continue;

                Console.WriteLine($"Clothe Name: ${clothe.ClotheName}");

                clothe.ProfileImagePath = _fileUploadService.GetFileUrl(clothe.ProfileImagePath);
            }

            var mappedRentals = _mapper.Map<PagedResult<RentalDTO>>(paginatedRentals);

            return Result<PagedResult<RentalDTO>>.Success(mappedRentals);
        }
    }
}
