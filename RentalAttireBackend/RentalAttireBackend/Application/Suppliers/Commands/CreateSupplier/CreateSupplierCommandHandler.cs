using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices;

namespace RentalAttireBackend.Application.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISupplierRepository _supplierRepo;
        private readonly IClotheRepository _clotheRepo;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditLogService _auditService;
        private readonly IUserRepository _userRepo;

        public CreateSupplierCommandHandler(
            IMapper mapper,
            ICurrentUserService currentUserService,
            ISupplierRepository supplierRepo,
            IClotheRepository clotheRepo,
            ITransactionManager transactionManager,
            IAuditLogService auditLogService,
            IUserRepository userRepo
            )
        {
            _mapper = mapper;
            _currentUserService = currentUserService;
            _clotheRepo = clotheRepo;
            _supplierRepo = supplierRepo;
            _transactionManager = transactionManager;
            _auditService = auditLogService;
            _userRepo = userRepo;
        }

        public async Task<Result<bool>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.FailureWithErrorType("Invalid request", ErrorType.BadRequest);

            try
            {
                var userId = _currentUserService.UserId;

                if (userId == 0 || userId is null)
                    return Result<bool>.FailureWithErrorType("Current user identifier does not exist", ErrorType.NotFound);

                var userWithEmployee = await _userRepo.GetUserWithEmployeeAsync(userId ?? 0, cancellationToken);

                if (userWithEmployee is null || userWithEmployee.Employee is null)
                    return Result<bool>.FailureWithErrorType("Employee associated with this transaction does not exist", ErrorType.NotFound);

                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var newSupplier = _mapper.Map<Supplier>(request);
                newSupplier.EmployeeId = userWithEmployee.Employee.Id;

                foreach(var clotheId in request.ClotheIds)
                {
                    var clothe = await _clotheRepo.GetClotheByIdAsync(clotheId, cancellationToken);

                    if (clothe is null)
                        continue;

                    clothe.Supplier = newSupplier;
                }

                var createSupplier = await _supplierRepo.CreateSupplierAsync(newSupplier, cancellationToken);

                if(!createSupplier)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Record could not be created. No changes were saved", ErrorType.BadRequest);
                }

                var auditTransaction = await _auditService.CreateAuditLogAsync(
                    newSupplier,
                    userWithEmployee.Id,
                    userWithEmployee.Person.FullName,
                    newSupplier.SupplierName
                    );

                if(!auditTransaction)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType(
                        "Failed to record audit log for this action. No changes were saved", 
                        ErrorType.BadRequest);
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Supplier created successfully.");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
