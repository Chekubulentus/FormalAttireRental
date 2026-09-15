using Microsoft.IdentityModel.Tokens;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.EntityRestorers.SupplierRestorer
{
    public class SupplierRestorer : IEntityRestorer
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITransactionManager _transactionManager;
        private readonly IUserRepository _userRepository;

        public SupplierRestorer(
            ISupplierRepository supplierRepository,
            IAuditLogService auditLogService,
            ICurrentUserService currentUserService,
            ITransactionManager transactionManager,
            IUserRepository userRepository
            )
        {
            _supplierRepository = supplierRepository;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
            _transactionManager = transactionManager;
            _userRepository = userRepository;
        }
        public string EntityType => "Supplier";

        public async Task<Result<bool>> RestoreAsync(int entityId, string performedBy, int performedById, CancellationToken ct)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;

                if (currentUserId == 0)
                    return Result<bool>.FailureWithErrorType("Current user identifier does not exist", ErrorType.BadRequest);

                var currentUser = await _userRepository.GetUserWithEmployeeAsync(currentUserId ?? 0, ct);

                var currentUserFullName = currentUser.Employee.User.Person.FullName;

                if (currentUser is null)
                    return Result<bool>.FailureWithErrorType("Current user not found", ErrorType.NotFound);

                var supplierToRestore = await _supplierRepository.GetSupplierWithNoRelationshipsByIdAsync(entityId, ct);

                if (supplierToRestore is null)
                    return Result<bool>.FailureWithErrorType("Supplier does not exist.", ErrorType.NotFound);

                await _transactionManager.BeginTransactionAsync(ct);

                supplierToRestore.IsActive = true;
                supplierToRestore.RestoredAt = DateTime.UtcNow;
                supplierToRestore.RestoredBy = currentUserFullName;

                var updateSupplier = await _supplierRepository.UpdateSupplieAsync(supplierToRestore, ct);

                if(!updateSupplier)
                {
                    await _transactionManager.RollbackTransactionAsync(ct);
                    return Result<bool>.FailureWithErrorType($"Supplier {supplierToRestore.SupplierName} could not be restored", ErrorType.BadRequest);
                }

                var auditAction = await _auditLogService.RestorationAuditLogAsync(
                    supplierToRestore,
                    currentUserFullName,
                    currentUser.Employee.Id,
                    supplierToRestore.SupplierName
                    );

                if(!auditAction)
                {
                    await _transactionManager.RollbackTransactionAsync(ct);
                    return Result<bool>.FailureWithErrorType("Failed to record audit log for this action. No changes were saved", ErrorType.BadRequest);
                }

                await _transactionManager.CommitTransacionAsync(ct);
                return Result<bool>.SuccessWithMessage($"Supplier successfully restored!");
            }
            catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(ct);
                throw;
            }
        }
    }
}
