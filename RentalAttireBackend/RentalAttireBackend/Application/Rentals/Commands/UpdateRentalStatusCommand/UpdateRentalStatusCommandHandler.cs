using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices;
using System.Transactions;

namespace RentalAttireBackend.Application.Rentals.Commands.UpdateRentalStatusCommand
{
    public class UpdateRentalStatusCommandHandler : IRequestHandler<UpdateRentalStatusCommmand, Result<bool>>
    {
        private readonly IRentalRepository _rentalRepo;
        private readonly IAuditLogService _auditService;
        private readonly ITransactionManager _transactionManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClotheRepository _clotheRepo;
        private readonly IUserRepository _userRepo;
        public UpdateRentalStatusCommandHandler(
            IRentalRepository rentalRepo,
            IAuditLogService auditService,
            ITransactionManager transactionManager,
            IHttpContextAccessor httpContextAccessor,
            IClotheRepository clotheRepo,
            IUserRepository userRepo
            )
        {
            _rentalRepo = rentalRepo;
            _auditService = auditService;
            _transactionManager = transactionManager;
            _httpContextAccessor = httpContextAccessor;
            _clotheRepo = clotheRepo;
            _userRepo = userRepo;

        }
        public async Task<Result<bool>> Handle(UpdateRentalStatusCommmand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.FailureWithErrorType("Invalid request. Please try again.", ErrorType.BadRequest);

            try
            {
                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var userIdClaim = _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst("userId")
                    ?.Value;

                if (!int.TryParse(userIdClaim, out int userId))
                    return Result<bool>.FailureWithErrorType("Current user does not exist.", ErrorType.NotFound);

                var currentUser = await _userRepo.GetUserWithEmployeeAsync(userId, cancellationToken);

                if (currentUser is null)
                    return Result<bool>.Failure("Current user could not be found.");
     
                var rental = await _rentalRepo.GetRentalByIdAsync(request.RentalId, cancellationToken);

                if (rental is null)
                    return Result<bool>.Failure("No rental reservation currently found.");

                var oldStatus = rental.Status;

                var updateRentalStatus = await ProcessRentalStatusConditionAsync(rental, request.Status, cancellationToken);

                if(!updateRentalStatus)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Invalid rental status condition.", ErrorType.BadRequest);
                }

                var updateRental = await _rentalRepo.UpdateRentalAsync(rental, cancellationToken);

                if(!updateRental)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Rental reservation status cannot be updated.");
                }

                var auditLogging = await _auditService.UpdateRentalReservationAsync(
                    rental,
                    oldStatus,
                    rental.RentalCode,
                    request.Status,
                    currentUser.Person.FullName,
                    currentUser.Id
                    );

                if(!auditLogging)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Failed to record audit log for this action. No changes were saved.");
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage($"Rental reservation {request.Status.ToLower()} successfully.");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }

        private async Task<bool> ProcessRentalStatusConditionAsync(Rental rental, string currentStatus, CancellationToken ct)
        {
            switch(currentStatus)
            {
                case "Confirmed":
                    foreach(var ri in rental.RentalItems)
                    {
                        var clothe = ri.Clothe;

                        clothe.AvailableQuantity = clothe.StockQuantity - clothe.ReservedQuantity;

                        await _clotheRepo.UpdateClotheAsync(clothe, ct);
                    }
                    rental.Status = "Confirmed";
                    return true;
                case "Declined":
                    rental.Status = "Declined";

                    foreach(var ri in rental.RentalItems)
                    {
                        var clothe = ri.Clothe;

                        clothe.ReservedQuantity -= ri.Quantity;
                        clothe.AvailableQuantity = clothe.StockQuantity - clothe.ReservedQuantity;

                        await _clotheRepo.UpdateClotheAsync(clothe, ct);
                    }
                    rental.Status = "Declined";
                    return true;
                case "Ready for pickup":
                    rental.Status = "Ready for pickup";
                    return true;

                case "Returned":

                    foreach(var ri in rental.RentalItems)
                    {
                        var clothe = ri.Clothe;

                        clothe.ReservedQuantity -= ri.Quantity;
                        clothe.AvailableQuantity = clothe.StockQuantity - clothe.ReservedQuantity;

                        await _clotheRepo.UpdateClotheAsync(clothe, ct);
                    }

                    rental.Status = "Returned";
                    return true;

                default:
                    return false;
            };
        }
    }
}
