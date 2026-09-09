using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RentalAttireBackend.Application.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISupplierRepository _supplierRepo;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditLogService _auditService;
        private readonly IUserRepository _userRepo;
        private readonly IClotheRepository _clotheRepository;

        public UpdateSupplierCommandHandler(
            IMapper mapper,
            ICurrentUserService currentUserService,
            ISupplierRepository supplierRepository,
            ITransactionManager transactionManager,
            IAuditLogService auditService,
            IUserRepository userRepo,
            IClotheRepository clotheRepository
            )
        {
            _mapper = mapper;
            _currentUserService = currentUserService;
            _supplierRepo = supplierRepository;
            _transactionManager = transactionManager;
            _auditService = auditService;
            _userRepo = userRepo;
            _clotheRepository = clotheRepository;
        }

        public async Task<Result<bool>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.FailureWithErrorType("Invalid request.", ErrorType.BadRequest);

            try
            {
                var userId = _currentUserService.UserId ?? 0;

                if (userId == 0)
                    return Result<bool>.FailureWithErrorType("Current user identifier does not exist", ErrorType.NotFound);

                var currentUser = await _userRepo.GetUserWithEmployeeAsync(userId, cancellationToken);

                if (currentUser is null)
                    return Result<bool>.FailureWithErrorType("Employee record does not exist", ErrorType.NotFound);

                var supplierToUpdate = await _supplierRepo.GetSupplierByIdAsync(request.SupplierId, cancellationToken);

                if (supplierToUpdate is null)
                    return Result<bool>.FailureWithErrorType("Supplier record could not be found", ErrorType.NotFound);

                var oldSupplierDetails = await _supplierRepo.GetSupplierByIdAsyncNoTracking(request.SupplierId, cancellationToken);

                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var clothesToBeAssigned = await _clotheRepository.GetClothesByIdsAsync(request.AssignClotheIds, cancellationToken);

                var alreadyAssignedClothes = clothesToBeAssigned.Where(c => c.SupplierId is not null).ToList();

                if(alreadyAssignedClothes.Any())
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    var names = string.Join(", ", alreadyAssignedClothes.Select(c => c.ClotheName));
                    return Result<bool>.FailureWithErrorType(
                        $"The following clothes are already assigned to another supplier: {names}",
                        ErrorType.BadRequest
                        );
                }

                foreach(var clothe in clothesToBeAssigned)
                {
                    clothe.SupplierId = supplierToUpdate.Id;
                }

                var clothesToUnassign = await _clotheRepository.GetClothesByIdsAsync(request.UnassignClotheIds, cancellationToken);

                var doesntBelongToSupplier = clothesToUnassign.Where(c => c.SupplierId != request.SupplierId).ToList();

                if(doesntBelongToSupplier.Any())
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    var names = string.Join(", ", doesntBelongToSupplier.Select(c => c.ClotheName));
                    return Result<bool>.FailureWithErrorType(
                        $"The following clothes does not belong to {oldSupplierDetails.SupplierName}: {names}",
                        ErrorType.BadRequest
                        );
                }

                foreach(var clothe in clothesToUnassign)
                {
                    clothe.SupplierId = null;
                }

                _mapper.Map(request, supplierToUpdate);

                var updateSupplier = await _supplierRepo.UpdateSupplieAsync(supplierToUpdate, cancellationToken);

                if(!updateSupplier)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Failed to update supplier details. No changes were saved.", ErrorType.BadRequest);
                }

                var auditAction = await _auditService.UpdateAuditLogAsync(
                    oldSupplierDetails,
                    supplierToUpdate,
                    currentUser.Employee.Id,
                    currentUser.Employee.User.Person.FullName,
                    supplierToUpdate.SupplierName
                    );

                if(!auditAction)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType(
                        "Failed to record audit log for this action. No changes were saved.", 
                        ErrorType.BadRequest);
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Supplier details updated successfully");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
