using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Rentals.DTOs;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Claims;

namespace RentalAttireBackend.Application.Rentals.Commands.RentalTransaction
{
    public class RentalTransactionCommandHandler : IRequestHandler<RentalTransactionCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditService;
        private readonly IRentalRepository _rentalRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITransactionManager _transactionManager;
        private readonly IClotheRepository _clotheRepo;
        private readonly IUserRepository _userRepo;
        public RentalTransactionCommandHandler(
            IMapper mapper,
            IAuditLogService auditService,
            IRentalRepository rentalRepo,
            ITransactionManager transactionManager,
            IClotheRepository clotheRepo,
            IUserRepository userRepo,
            ICurrentUserService currentUserService
            )
        {
            _mapper = mapper;
            _auditService = auditService;
            _rentalRepo = rentalRepo;
            _clotheRepo = clotheRepo;
            _userRepo = userRepo;
            _transactionManager = transactionManager;
            _currentUserService = currentUserService;
        }
        public async Task<Result<bool>> Handle(RentalTransactionCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request.");
            
            try
            {
                var userIdClaim = _currentUserService.UserId;

                if (userIdClaim is null || userIdClaim == 0)
                    return Result<bool>.FailureWithErrorType("User does not exist.", ErrorType.NotFound);

                var customerUser = await _userRepo.GetUserByIdWithCustomerAsync(userIdClaim.Value, cancellationToken);

                if (customerUser is null || customerUser.Customer is null)
                    return Result<bool>.FailureWithErrorType("Customer record does not exist.", ErrorType.NotFound);

                var rentalItems = _mapper.Map<List<RentalItem>>(request.RentalItems);

                var clothesToUpdate = new List<Clothe>();

                await _transactionManager.BeginTransactionAsync(cancellationToken);

                foreach (var ri in rentalItems)
                {
                    var clothe = await _clotheRepo.GetClotheByIdAsync(ri.ClotheId, cancellationToken);

                    if (clothe is null)
                        return Result<bool>.FailureWithErrorType("One or more selected items are no longer available.", ErrorType.BadRequest);

                    if (ri.Quantity <= 0)
                        return Result<bool>.FailureWithErrorType($"{clothe?.ClotheName} quantity must be greater than zero.", ErrorType.BadRequest);

                    if (ri.Quantity > clothe.AvailableQuantity)
                        return Result<bool>.FailureWithErrorType($"{clothe.ClotheName} does not have enough available stock for you reservation.", ErrorType.BadRequest);

                    ri.RentalPrice = clothe.RentalPrice;
                    clothe.ReservedQuantity += ri.Quantity;
                    clothe.AvailableQuantity = clothe.StockQuantity - clothe.ReservedQuantity;

                    clothesToUpdate.Add(clothe);
                }

                var rental = _mapper.Map<Rental>(request);

                rental.CustomerId = customerUser.Customer.Id;
                rental.TotalAmount = rentalItems.Sum(ri => ri.TotalAmount);
                rental.DepositAmount = rentalItems.Sum(ri => ri.DepositAmount * ri.Quantity);
                rental.Status = "Pending";
                rental.RentalItems = rentalItems;

                var createRental = await _rentalRepo.CreateRentalAsync(rental, cancellationToken);

                if(!createRental)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Reservation failed. Please try again.", ErrorType.BadRequest);
                }

                foreach(var item in clothesToUpdate)
                {
                    var updateClothe = await _clotheRepo.UpdateClotheAsync(item, cancellationToken);

                    if(!updateClothe)
                    {
                        await _transactionManager.RollbackTransactionAsync(cancellationToken);
                        return Result<bool>.FailureWithErrorType("Failed to update clothe availability.", ErrorType.BadRequest);
                    }
                }

                var reservationLogging = await _auditService.RentalReservationAuditLogAsync(
                    rental,
                    customerUser.Person.FullName,
                    customerUser.Customer.Id,
                    rental.RentalCode
                    );

                if (!reservationLogging)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Failed to create audit log for this action. No changes were saved.", ErrorType.BadRequest);
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Your rental reservation has been submitted and is pending confirmation.");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.FailureWithErrorType(e.Message, ErrorType.BadRequest);
            }
        }
    }
}
