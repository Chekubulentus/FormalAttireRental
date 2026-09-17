using MediatR;
using Microsoft.IdentityModel.Tokens;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.CompilerServices;

namespace RentalAttireBackend.Application.Suppliers.Commands.ArchiveSupplier
{
    public class ArchiveSupplierByIdCommandHandler : IRequestHandler<ArchiveSupplierByIdCommand, Result<bool>>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public ArchiveSupplierByIdCommandHandler(
            ITransactionManager transactionManager,
            ISupplierRepository supplierRepository,
            IAuditLogService auditLogService,
            ICurrentUserService currentUserService,
            IUserRepository userRepository
            )
        {
            _transactionManager = transactionManager;
            _supplierRepository = supplierRepository;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<Result<bool>> Handle(ArchiveSupplierByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;

                if (currentUserId == 0)
                    return Result<bool>.FailureWithErrorType("Current user identifier doesn't exist", ErrorType.BadRequest);

                var currentUser = await _userRepository.GetUserWithEmployeeAsync(currentUserId ?? 0, cancellationToken);

                if (currentUser is null)
                    return Result<bool>.FailureWithErrorType("Current user record doesn't exist", ErrorType.NotFound);

                var supplier = await _supplierRepository.GetSupplierWithNoRelationshipsByIdAsync(request.Id, cancellationToken);

                if (supplier is null)
                    return Result<bool>.FailureWithErrorType("Supplier record does not exist.", ErrorType.NotFound);

                await _transactionManager.BeginTransactionAsync(cancellationToken);

                supplier.IsActive = false;
                supplier.ArchivedAt = DateTime.UtcNow;
                supplier.ArchivedBy = currentUser.Employee.User.Person.FullName;

                var updateSupplier = await _supplierRepository.UpdateSupplieAsync(supplier, cancellationToken);

                if(!updateSupplier)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Supplier could not be archived", ErrorType.BadRequest);
                }

                var auditAction = await _auditLogService.ArchiveAuditLogAsync(
                    supplier,
                    currentUser.Id,
                    currentUser.Employee.User.Person.FullName,
                    supplier.SupplierName
                    );

                if(!auditAction)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Failed to record audit log for this action. No changes were saved", ErrorType.BadRequest);
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage($"Supplier {supplier.SupplierName} is successfully archived!");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
