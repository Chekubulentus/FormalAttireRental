using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Security.Claims;

namespace RentalAttireBackend.Application.Rentals.Commands.RentalTransaction
{
    public class RentalTransactionCommandHandler : IRequestHandler<RentalTransactionCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditService;
        private readonly IRentalRepository _rentalRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITransactionManager _transactionManager;
        private readonly IClotheRepository _clotheRepo;
        private readonly IUserRepository _userRepo;
        public RentalTransactionCommandHandler(
            IMapper mapper,
            IAuditLogService auditService,
            IRentalRepository rentalRepo,
            IHttpContextAccessor httpContextAccessor,
            ITransactionManager transactionManager,
            IClotheRepository clotheRepo,
            IUserRepository userRepo
            )
        {
            _mapper = mapper;
            _auditService = auditService;
            _rentalRepo = rentalRepo;
            _httpContextAccessor = httpContextAccessor;
            _clotheRepo = clotheRepo;
            _userRepo = userRepo;
        }
        public async Task<Result<bool>> Handle(RentalTransactionCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request.");

            if (!request.RentalItems.Any() || request.RentalItems.Count() == 0)
                return Result<bool>.Failure("No items currently selected.");

            if (string.IsNullOrEmpty(request.PaymentMethod))
                return Result<bool>.Failure("Please choose a payment method.");

            if (string.IsNullOrEmpty(request.GcashRefNum) || string.IsNullOrEmpty(request.GcashRefName))
                return Result<bool>.Failure("Please proceed first with payment.");

            try
            {
                var userIdClaim = _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirst("userId")?
                    .Value;

                if (!int.TryParse(userIdClaim, out var userId))
                    return Result<bool>.Failure("Unable to identify current user.");

                var customerUser = await _userRepo.GetUserByIdWithCustomerAsync(userId, cancellationToken);

                if (customerUser is null || customerUser.Customer is null)
                    return Result<bool>.Failure("Customer record does not exist.");

                var rentalItems = _mapper.Map<List<RentalItem>>(request.RentalItems);

                foreach(var ri in rentalItems)
                {
                    var clothe = await _clotheRepo.GetClotheByIdAsync(ri.ClotheId, cancellationToken);

                    if (clothe is null)
                        return Result<bool>.Failure("One or more selected items are no longer available.");

                    if (ri.Quantity > clothe.AvailableQuantity)
                        return Result<bool>.Failure($"{clothe.ClotheName} does not have enough available stock");

                    ri.RentalPrice = clothe.RentalPrice;
                }

                var rental = _mapper.Map<Rental>(request);

                rental.CustomerId = customerUser.Customer.Id;
                rental.TotalAmount = rentalItems.Sum(ri => ri.TotalAmount);
                rental.DepositAmount = rentalItems.Sum(ri => ri.DepositAmount * ri.Quantity);
                rental.Status = "Pending";
                rental.RentalItems = rentalItems;

                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var createRental = await _rentalRepo.CreateRentalAsync(rental, cancellationToken);

                if(!createRental)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Reservation failed. Please try again.");
                }

                // KULANG PA NG AUDIT LOGGING!!!!

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Your rental reservation has been submitted and is pending confirmation.");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
