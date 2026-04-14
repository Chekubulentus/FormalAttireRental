using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace RentalAttireBackend.Application.Customers.Commands.ArchiveCustomer
{
    public class ArchiveCustomerByIdCommandHandler : IRequestHandler<ArchiveCustomerByIdCommand, Result<bool>>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly ICustomerRepository _customerRepo;
        private readonly IAuditLogService _auditService;

        public ArchiveCustomerByIdCommandHandler(
            ITransactionManager transactionManager,
            ICustomerRepository customerRepo,
            IAuditLogService auditService
            )
        {
            _transactionManager = transactionManager;
            _customerRepo = customerRepo;
            _auditService = auditService;
        }

        public async Task<Result<bool>> Handle(ArchiveCustomerByIdCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request.");

            if (request.Id == 0)
                return Result<bool>.Failure("Customer identifier does not exist.");

            try
            {
                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var customer = await _customerRepo.GetCustomerByIdAsync(request.Id, cancellationToken);

                if (customer is null || customer.User is null)
                    return Result<bool>.Failure("Customer does not exist.");

                customer.IsActive = false;
                customer?.User?.IsActive = false;
                customer?.User?.Person.IsActive = false;

                var updateCustomer = await _customerRepo.UpdateCustomerAsync(customer!, cancellationToken);

                if(!updateCustomer)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Customer could not be archived.");
                }

                var auditTransaction = await _auditService.ArchiveAuditLogAsync(
                    customer!,
                    request.PerformedById,
                    request.PerformedBy,
                    customer!.User.Person.FullName
                    );

                if(!auditTransaction)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Transaction could not be audited.");
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Customer successfully archived.");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
