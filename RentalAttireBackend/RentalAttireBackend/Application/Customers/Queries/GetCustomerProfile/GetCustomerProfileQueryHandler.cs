using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Customers.Queries.GetCustomerProfile
{
    public class GetCustomerProfileQueryHandler : IRequestHandler<GetCustomerProfileQuery, Result<CustomerDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        private readonly ICurrentUserService _currentUserService;

        public GetCustomerProfileQueryHandler(
            IMapper mapper,
            IUserRepository userRepo,
            ICurrentUserService currentUserService
            )
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _currentUserService = currentUserService;
        }
        public async Task<Result<CustomerDTO>> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId == 0)
                return Result<CustomerDTO>.FailureWithErrorType("User identifier claim could not be fetched.", ErrorType.NotFound);

            var user = await _userRepo.GetUserByIdWithCustomerAsync(userId ?? 0, cancellationToken);

            if (user is null)
                return Result<CustomerDTO>.FailureWithErrorType("Customer record does not exist.", ErrorType.NotFound);

            var customerDto = _mapper.Map<CustomerDTO>(user.Customer);

            return Result<CustomerDTO>.Success(customerDto);
        }
    }
}
