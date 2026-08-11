using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
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
                return Result<bool>.Failure("Invalid request. Please try again.");

            if (request.RentalId <= 0)
                return Result<bool>.Failure("Rental reservation could not be found.");

            if (request.Status != "Confirmed" && request.Status != "Declined")
                return Result<bool>.Failure("Invalid rental status.");

            try
            {
                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var userIdClaim = _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst("userId")
                    ?.Value;

                if (!int.TryParse(userIdClaim, out int userId))
                    return Result<bool>.Failure("Current user does not exist.");

                var currentUser = await _userRepo.GetUserWithEmployeeAsync(userId, cancellationToken);

                if (currentUser is null)
                    return Result<bool>.Failure("Current user could not be found.");

                var rental = await _rentalRepo.GetRentalByIdAsync(request.RentalId, cancellationToken);

                if (rental is null)
                    return Result<bool>.Failure("No rental reservation currently found.");

                var oldStatus = rental.Status;

                if (!rental.Status.Equals("Pending"))
                    return Result<bool>.Failure("Only pending reservations can be updated.");

                if(request.Status.Equals("Confirmed"))  
                {
                    foreach(var ri in rental.RentalItems)
                    {
                        var clothe = ri.Clothe;

                        clothe.ReservedQuantity -= ri.Quantity;
                        clothe.StockQuantity -= ri.Quantity;
                        clothe.AvailableQuantity = clothe.StockQuantity - clothe.ReservedQuantity;

                        await _clotheRepo.UpdateClotheAsync(clothe, cancellationToken);
                    }
                    rental.Status = "Confirmed";
                }

                if(request.Status.Equals("Declined"))
                {
                    foreach(var ri in rental.RentalItems)
                    {
                        var clothe = ri.Clothe;

                        clothe.ReservedQuantity -= ri.Quantity;

                        clothe.AvailableQuantity = clothe.StockQuantity - clothe.ReservedQuantity;

                        await _clotheRepo.UpdateClotheAsync(clothe, cancellationToken);
                    }

                    rental.Status = "Declined";
                }

                var updateStatus = await _rentalRepo.UpdateRentalAsync(rental, cancellationToken);

                if(!updateStatus)
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
                return Result<bool>.SuccessWithMessage($"Rental reservation ${request.Status.ToLower()} successfully.");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
